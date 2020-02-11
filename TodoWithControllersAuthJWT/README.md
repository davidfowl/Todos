Added JWT for local authentication and authorization.

Now, TodoWithControllersAuthJWT project is available only for users within Program.validUsers dictionary.

All methods from /api/todos (except GenerateToken) will require token validation
More than that, 
- GET /api/todos/{id} will require "user" policy, that is defined as "can_view" claim
- DELETE /api/todos/{id} will require "admin" policy, that is defined as "can_delete" claim

For demo purposes, Claims are requested on GenerateToken endpoint.
```
# pwsh
$base = "http://localhost:8888"
$payload = "username=user&password=123456"
$auth = Invoke-RestMethod "$base/api/auth/GenerateToken?$payload" -Method Get
# $auth should contains token property
$info = Invoke-RestMethod "$base/api/todos/2" -Headers @{"Authorization" = "Bearer $($auth.token)"}
```