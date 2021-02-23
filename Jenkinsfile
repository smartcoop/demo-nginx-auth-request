node('master') {
	if (env.BRANCH_NAME.contains("authrequest/")){
		stage('Git checkout') {
		checkout scm
	  }
	  stage('Docker build') {
		sh "chmod 777 ./AuthenticationServer/jenkins-deploy.sh"
		sh "./AuthenticationServer/jenkins-deploy.sh"
		sh "chmod 777 ./VerySimpleAPI/jenkins-deploy.sh"
		sh "./VerySimpleAPI/jenkins-deploy.sh"
	  }
	  stage('clean up our workspace') {
		deleteDir()
		dir("${workspace}@tmp") {
		  deleteDir()
		}
	  }
	}
}