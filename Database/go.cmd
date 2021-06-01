docker build -t tsdb .
docker run -p 1433:1433 -d tsdb --name tomsharpdb