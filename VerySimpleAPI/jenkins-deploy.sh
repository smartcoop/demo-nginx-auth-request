#! /usr/bin/env bash

# This scripts is called by Jenkins whenever a branch is pushed to GitHub. See
# the README.md file.

set -e

# Convert uppercase to lowercase.
DOCKER_NAME=$(echo verysimpleapi_${BRANCH_NAME} | sed -e 's/\(.*\)/\L\1/' -e 's/origin\/master/master/g' -e 's/origin\/feature\//feature-/g' -e 's/origin\/hotfix\//hotfix-/g' -e 's/feature\//feature-/g' -e 's/hotfix\//hotfix-/g' -e 's/authrequest\///g')
DOCKER_HOSTNAME=$(echo ${BRANCH_NAME} | sed -e 's/\(.*\)/\L\1/')
echo 
if ! echo ${DOCKER_NAME} |  grep -q "^[a-z][a-z0-9_-]*$"; then
  echo "The branch does not use only alphanumeric characters and dashes. Exiting."; 
  exit 1
fi

# TODO HTML-escape those strings, especially the subject.
GIT_COMMIT="$(git rev-parse HEAD)"
GIT_COMMIT_SUBJECT="$(git log -n1 --format=format:"%s")"
GIT_COMMIT_DATE="$(git log -n1 --format=format:"%ci")"
GIT_AUTHOR_NAME="$(git log -n1 --format=format:"%aN")"
GIT_AUTHOR_EMAIL="$(git log -n1 --format=format:"%cE")"

echo "Starting jenkins-deploy.sh script...."
echo "Branch name: ${DOCKER_HOSTNAME}"
echo "Domain: ${DOCKER_HOSTNAME}.smartcoop.dev"
echo "Git commit: ${GIT_COMMIT}"
echo "Git subject: ${GIT_COMMIT_SUBJECT}"
echo "Git date: ${GIT_COMMIT_DATE}"
echo "Git author name: ${GIT_AUTHOR_NAME}"
echo "Git author email: ${GIT_AUTHOR_EMAIL}"

docker build \
  --build-arg GIT_BRANCH_NAME="${DOCKER_HOSTNAME}"\
  --build-arg GIT_COMMIT="${GIT_COMMIT}"\
  --build-arg GIT_COMMIT_SUBJECT="${GIT_COMMIT_SUBJECT}"\
  --build-arg GIT_COMMIT_DATE="${GIT_COMMIT_DATE}"\
  --build-arg GIT_AUTHOR_NAME="${GIT_AUTHOR_NAME}"\
  --build-arg GIT_AUTHOR_EMAIL="${GIT_AUTHOR_EMAIL}"\
  -t ${DOCKER_NAME}\
  ./VerySimpleAPI/

docker rm -f ${DOCKER_NAME} || true

docker run -d \
  --name ${DOCKER_NAME} \
  --restart always \
  --network jenkins \
  ${DOCKER_NAME}

echo "Deployment done."