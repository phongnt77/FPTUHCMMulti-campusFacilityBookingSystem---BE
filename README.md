# Chuẩn viết code RESTful API cho C\#

## 1. Cấu trúc dự án

```txt
src/
 ├── Controller/       # Xử lý request, response
 ├── BLL/                # Định nghĩa data model
 ├── DAL/                # Xử lý logic, gọi DB
 ├── Applications/       # Helpers
 │     └── utils/
 │     └── mappers/
 │     └── DTOs/
 │     └── auth/
 │     └── …
```

## 2. Quy tắc đặt tên endpoint

* Dùng **danh từ số nhiều** (plural nouns).
* **Không** nhúng hành động trong URL (`/api/users/create` ❌).
* Action được quyết định bằng **HTTP verb**.

Ví dụ cho resource `User`:

| HTTP Verb | Endpoint        | Mô tả                     |
| --------- | --------------- | ------------------------- |
| GET       | /api/users      | Lấy danh sách user        |
| GET       | /api/users/{id} | Lấy chi tiết user theo id |
| POST      | /api/users      | Tạo user mới              |
| PUT       | /api/users/{id} | Cập nhật toàn bộ user     |
| PATCH     | /api/users/{id} | Cập nhật một phần user    |
| DELETE    | /api/users/{id} | Xóa user                  |

👉 Nếu có sub-resource:

```bash
GET /api/users/1/posts        # Lấy tất cả bài post của user 1
GET /api/users/1/posts/99     # Lấy chi tiết post 99 của user 1
```

## 3. Quy tắc đặt tên Controller

* PascalCase + suffix `Controller`.
* Tên controller khớp với resource.
* ASP.NET Core mặc định map: `UsersController` → `/api/users`.

Ví dụ:

```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    // GET /api/users
    [HttpGet]
    public async Task<IActionResult> GetAllUsers() { ... }

    // GET /api/users/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id) { ... }

    // POST /api/users
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto) { ... }

    // PUT /api/users/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto) { ... }

    // DELETE /api/users/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id) { ... }
}
```

## 4. Quy tắc DTO & Model

* **Entity (DB model):** PascalCase, số ít → `User`.
* **DTO:** PascalCase + suffix `Dto` → `CreateUserDto`, `UpdateUserDto`.
* **Interface:** PascalCase, prefix `I` → `IUserService`.

Ví dụ:

```csharp
public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}
```

## 5. Error Handling & Validation

* Dùng **ModelState** để validate input.
* Trả về mã lỗi chuẩn: `400 BadRequest`, `401 Unauthorized`, `404 NotFound`, `500 InternalServerError`.
* Middleware global để handle exception.
* Tạo 1 class chung để phân loại các request.

Ví dụ Validation:

```csharp
[HttpPost]
public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(new {
            success = false,
            error = new { code = 400, message = "Invalid request data" }
        });
    }

    var user = await _userService.CreateUserAsync(dto);
    return Ok(new { success = true, data = user });
}
```

## 6. Code Style

* Dùng `async/await` cho tất cả API call tới DB.
* Controller chỉ xử lý request/response, logic chính đặt trong Service.
* Request validation bằng **FluentValidation** hoặc **DataAnnotation**.
* Error handling qua **Middleware chung**.

```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new {
                success = false,
                error = new { code = 404, message = "User not found" }
            });
        }
        return Ok(new { success = true, data = user });
    }
}
```

## 7. Quy tắc khác

* Tên phương thức trong Controller: PascalCase (`GetUserById`).
* Không viết logic trong Controller → tách sang `Service`.
* Sử dụng *async/await* cho tất cả thao tác DB/IO.
* Swagger/OpenAPI để mô tả API.
* Thêm Service Register để không cần khai báo nhiều trong `program.cs`.
* Mỗi Service/Repo cần 1 Interface riêng.

---

# Chuẩn JSON trả về (API Response)

## 1. Thành công

```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Nguyen Van A"
  }
}
```

## 2. Lỗi

```json
{
  "success": false,
  "error": {
    "code": 404,
    "message": "User not found"
  }
}
```

## 3. Danh sách (có phân trang)

```json
{
  "success": true,
  "data": [
    { "id": 1, "name": "A" },
    { "id": 2, "name": "B" }
  ],
  "pagination": {
    "page": 1,
    "limit": 10,
    "total": 52
  }
}
```
