# Very Simple API

This project is a very small API (coded in .NET Core) that returns a text given in URL.

For example: `xxx.smartcoop.dev/api/yyy` where 

- `xxx` is a branch name like `feature-simple-api` (`feature/` is replaced by `feature-`)
- `yyy` is the text to show in the webpage

# What was the purpose of this "exercice" ?

The purpose of this work is to show how to automatically deploy a .NET Core API project in a Docker container and how to make it accessible through a URL.

# Automated deployment

## Jenkins scan

Jenkins scans the "smart-back-end" repository to check if something change on a existing branch or if this is a new branch.
If the scan finds a file named `Jenkinsfile`, the script contained in this file will be executed.
This file is stored at the root directory of the solution.

### The interesting part of the "Jenkinsfile" script for this project

Explanations are available in the script below ([full script](/Jenkinsfile))

```
node('master') { // On Jenkins server
  // The checkout step will checkout code from source control.
  // scm is a special variable which instructs the checkout step to clone the specific revision which triggered this Pipeline run.
  stage('Git checkout') { 
    checkout scm
  }
  // Docker Build Step
  // This step launches the "jenkins-deploy.sh" script contained in the application directory
  stage('Docker build') {
    sh "chmod 777 ./Apps/VerySimpleAPI/jenkins-deploy.sh"
    sh "./Apps/VerySimpleAPI/jenkins-deploy.sh"
  }
  // Notify step
  // This step is used to add notifications in Github (e.g. information from SonarQube).
  stage('Notify GitHub') {
    sh "chmod 777 ./scripts/smart-github-api/run.sh"
	sh "chmod 777 ./scripts/smart-github-api/get-sonar-results.sh"
	sh "chmod 777 ./scripts/smart-github-api/install.sh"
	sh "chmod 777 ./scripts/smart-github-api/push-sonar-results.sh"
    sh "./scripts/smart-github-api/run.sh"
  }
  // Cleaning step
  // This step is used to suppress temporary workspace used.
  stage('clean up our workspace') {
    deleteDir()
    dir("${workspace}@tmp") {
      deleteDir()
    }
  }
}
```

## jenkins-deploy.sh

This [script](jenkins-deploy.sh) is located in the root directory of the application.
It is used to:

- format the branch name in a compliant Docker container name (Warning: the Docker container name is prefixed);
- convert uppercase to lowercase in the names used by the script;
- get commit information about the branch being deployed;
- build a Docker image containing the application to deploy;
- delete the old Docker instance if it exists;
- run a new Docker instance with the new branch/commit.



## NGINX

A Docker container named `nginx` is running on the server `SRVUJENKINS` (`172.16.1.105`).
This container is used like a reverse proxy to publish content from other Docker containers (PR of documentation for example).
The config files of this container are located in the directory `/home/smjenkins/jenkins/nginx/conf`.
In this directory, we can find the interesting file `smartcoop.dev.conf`.

### What do we find in the file `smartcoop.dev.conf` ?

Warning: the example below is a snapshot at one moment and only used for explanations. Explanations are available in the script below.

```
server {
    # NGINX listens on port 80
    # All requests on http://XXX.smartcoop.dev (port 80) will be redirected to https://XXX.smartcoop.dev (port 443)
    # with permanent redirection doc
    listen 80;
    listen [::]:80;
    server_name ~^(?<name>[a-z][a-z0-9-]*)\.smartcoop\.dev$;
    return 301 https://$host$request_uri;

}

server {
  # NGINX listens on port 443
  # The domain `.smartcoop.dev` is binded to a wildcard certificate (certificate for multiple subdomains)
  # All requests on doc_YYY.smartcoop.dev will be send to the Docker container named `doc_YYY`
  # All requests on verysimpleapi_YYY.smartcoop.dev/api will be send to the Docker container named `verysimpleapi_YYY`
  listen 443 ssl;
  server_name ~^(?<name>[a-z][a-z0-9-]*)\.smartcoop\.dev$;
  ssl_certificate /etc/nginx/ssl/smartcoop.dev.cer;
  ssl_certificate_key /etc/nginx/ssl/smartcoop.dev.key;
  resolver 127.0.0.11 ipv6=off;

        location /api {
          proxy_redirect  off;
          # $name is intended to be the hostname of a Docker container on the same Docker network than this Nginx instance.
          proxy_pass http://verysimpleapi_$name:80;
        }
        location / {
          proxy_redirect  off;
          # $name is intended to be the hostname of a Docker container on the same Docker network than this Nginx instance.
          proxy_pass http://doc_$name:80;
        }

        location ~ /\. {
          deny all;
        }

        error_page 502 /custom_502.html;
          ssi on;
          location = /custom_502.html {
          root /usr/share/nginx/html;
          internal;
        }
}
```
# Almost automatic Docker container removing

When a branch is deleted, a script can be launched to stop and remove the Docker container used.
This script is contained in a [Jenkins job](https://jenkins.smartbe.be/job/cleanDockerJenkinsVerySimpleAPI/) named `cleanDockerJenkinsVerySimpleAPI`.

## What do we find in this script

The first part of the script is only composed by some `echo` commands to display information.
The most interesting part of this script is the command `docker rm -f` used to stop and remove the Docker container.

```
echo "================================================"
echo "BRANCH_NAME: ${SOURCE_PROJECT_NAME}"
echo "================================================"
docker rm -f $(echo verysimpleapi_${SOURCE_PROJECT_NAME} | sed -e 's/\//-/g' -e 's/%2F/-/g') || true
```