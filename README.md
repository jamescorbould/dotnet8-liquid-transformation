Testing locally using curl (example):
```
curl -s -H "Content-Type: application/json" -d '{"name": "James"}' http://localhost:7071/api/Function | jq .
```

This returns back the transformed JSON rendered by the Liquid templating engine:
```
{
  "kimName": "James"
}
```
