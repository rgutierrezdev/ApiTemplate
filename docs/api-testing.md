# API Testing

APIs can be tested vía ThunderClient, Postman as well as the built in SwaggerUI or Redoc.

Once the API is up and running, here are the ways to test the functionality easily.

Remember to enable Swagger in your API, see [Configurations](configurations.md#swagger)

## Swagger UI

Navigate to your API url `https://localhost:<port>/swagger`

![Swagger Screenshot](assets/swagger1.jpg)

## Redoc

Navigate to your API url `https://localhost:<port>/redoc`

![Redoc Screenshot](assets/redoc.jpg)

Here you can see a very detailed definition of all available endpoints, but you cannot test them from here.

## ThunderClient

This extension will be very helpful to you too if you use Visual Code for .NET development. Make sure that you have the
ThunderClient VsCode Extension installed

You just have to import the API specification already created by Swagger and import it from the API
url `https://localhost:<port>/swagger/v1/swagger.json`

![Thunder Client Screenshot](assets/thunderclient1.jpg)

## Postman

Postman is also supported. Same way as ThunderClient, you just have to import the API Specification into Postman using
the API url `https://localhost:<port>/swagger/v1/swagger.json`

![Postman Screenshot](assets/postman1.jpg)
