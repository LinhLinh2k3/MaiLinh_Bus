
# Project Title

🚍 Bus Management System – Mai Linh

A graduation project that builds a comprehensive bus management system for Mai Linh company, aiming to digitize ticket booking and streamline operations.

📊 Achievements:

Optimized manual operations into a digital platform.

Reduced booking errors, improved efficiency, and enhanced customer experience.

Designed with scalability and data security in mind.


## Features

👥 Customer: Online ticket booking, cancel/change seats, view history, make payments (VNPay, ZaloPay), receive e-tickets via email, and give feedback.

👨‍💼 Staff: Manage customers, approve ticket changes, issue invoices, publish news/announcements.

🛠 Admin: Manage buses, drivers, routes, employees, promotions, and analyze revenue with statistical reports.
## Tech Stack

**Client:** Html5, TailwindCSS, Javascript, RESTful API, AJAX

**Server:** C#, Javascript, RESTful API, Entity Framework, MVC model

**Database:** SQL Server

**API:** Gemini, ZaloPay, MoMo, VNPay


## API Reference

#### PostPost all items

```http
  POST /v2/create
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `app\_id`        | int    | **Required**. App ID do ZaloPay cung cấp   |
| `app\_trans\_id` | string | **Required**. Mã giao dịch của merchant    |
| `app\_time`      | long   | **Required**. Thời gian tạo giao dịch (ms) |
| `app\_user `     | string | **Required**. Người dùng khởi tạo          |
| `amount`         | long   | **Required**. Số tiền thanh toán           |
| `description`    | string | **Required**. Mô tả giao dịch              |
| `embed\_data`    | string | Thông tin bổ sung (JSON string)            |
| `item`           | string | Danh sách sản phẩm (JSON string)           |
| `mac`            | string | **Required**. HMAC SHA256 của dữ liệu      |
| `callback\_url`  | string | **Required**. URL nhận kết quả thanh toán  |

#### Get all item

```http
  POST /v2/gateway/api/query
```

| Parameter   | Type   | Description                        |
| ----------- | ------ | ---------------------------------- |
| `partnerCode` | string | **Required**. Mã đối tác MoMo cấp  |
| `requestId `  | string | **Required**. Mã request duy nhất  |
| `orderId `    | string | **Required**. Mã đơn hàng          |
| `lang `       | string | Ngôn ngữ phản hồi (vi/en)          |
|` signature `  | string | **Required**. Chuỗi chữ ký bảo mật |



```http
  POST /v2/gateway/api/create
```

| Parameter   | Type   | Description                                  |
| ----------- | ------ | -------------------------------------------- |
|` partnerCode` | string | **Required**. Mã đối tác MoMo cấp            |
| `orderId`     | string | **Required**. Mã đơn hàng duy nhất           |
| `requestId`   | string | **Required**. Mã request duy nhất            |
| `amount`      | long   | **Required**. Số tiền thanh toán             |
| `orderInfo `  | string | **Required**. Nội dung đơn hàng              |
| `redirectUrl` | string | **Required**. URL trả về khi thanh toán xong |
| `ipnUrl`      | string | **Required**. URL callback (IPN)             |
| `extraData `  | string | Dữ liệu bổ sung (base64)                     |
| `requestType` | string | Loại yêu cầu (captureWallet, payWithATM, …)  |
| `signature`   | string | **Required**. Chuỗi chữ ký bảo mật           |

```http
   POST /v1/chat/completions
```
| Parameter   | Type   | Description                                       |
| ----------- | ------ | ------------------------------------------------- |
| `model     `  | string | **Required**. Tên model Gemini (vd: gemini-pro)   |
| `messages`    | array  | **Required**. Danh sách tin nhắn (role + content) |
| `temperature` | float  | Nhiệt độ sáng tạo (0–1, mặc định 0.7)             |
| `top\_p`      | float  | Tỷ lệ xác suất hạt nhân (mặc định 1)              |
| `max\_tokens `| int    | Giới hạn token sinh                               |
| `api\_key`    | string | **Required**. Khóa API của Gemini                 |


```http
    GET /payment/vnpay
```
| Parameter       | Type   | Description                                       |
| --------------- | ------ | ------------------------------------------------- |
| `vnp\_Version`    | string | **Required**. Phiên bản API (2.1.0)               |
| `vnp\_Command `   | string | **Required**. Loại giao dịch (pay, refund…)       |
| `vnp\_TmnCode`    | string | **Required**. Mã terminal do VNPay cấp            |
| `vnp\_Amount`     | long   | **Required**. Số tiền (nhân 100)                  |
| `vnp\_CreateDate` | string | **Required**. Ngày tạo giao dịch (yyyyMMddHHmmss) |
| `vnp\_CurrCode`   | string | Loại tiền tệ (VND)                                |
| `vnp\_IpAddr`     | string | IP của khách hàng                                 |
| `vnp\_Locale`     | string | Ngôn ngữ (vn/en)                                  |
| `vnp\_OrderInfo`  | string | **Required**. Nội dung đơn hàng                   |
| `vnp\_ReturnUrl`  | string | **Required**. URL nhận kết quả                    |
| `vnp\_TxnRef`     | string | **Required**. Mã giao dịch                        |
| `vnp\_SecureHash` | string | **Required**. Chuỗi chữ ký bảo mật                |



## DEV
 * Ngô Thị Thùy Linh: team leader, systems analyst, database, code  

 * Nguyễn Ngọc Quân: team memeber, code

 * Đặng Hữu Chiến: team memeber, code 
