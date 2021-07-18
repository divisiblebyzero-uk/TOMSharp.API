docker build -t tsdb .
docker run -p 1433:1433 --name tomsharpdb -d tsdb