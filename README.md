# tribal-interview
Repo for Tribal Take Home Test


The microservice was built using:
	AWS Lambda => Serverless function that runs the microservice code.
	AWS API Gateway => To Map an URL to the Lambda.
	AWS DynamoDB => To store the credit line applications and the correspondent response.
	AWS CloudWatch => To monitor logs about the Lambda.


The code has 3 main Layers:
	Infraestructure => In this layer we should create data access or api access logic.
	Model => In this layer we should create the entities(Inputs/ Outputs / Mappings to DB / Domain Entities).
	Services => In this layer we should create the services that will handle the business logic.


The application is already deployed to my personal AWS account in this URL: https://ism1mswe99.execute-api.us-east-1.amazonaws.com/Prod/
