# BasicWebServer – инструкции

## Структура на решението

- **ServerWeb** (Class Library, net9.0) – HTTP сървър и обща логика  
  - `Common/Guard.cs` – валидация срещу null  
  - `HTTP/` – Request, Response модели, Header, HeaderCollection, Cookie, CookieCollection, Session, Method, StatusCode, ContentType  
  - `Responses/` – Response, ContentResponse, TextResponse, HtmlResponse, RedirectResponse, BadRequestResponse, NotFoundResponse, UnauthorizedResponse, TextFileResponse  
  - `Routing/` – IRoutingTable, RoutingTable  
  - `HttpServer.cs` – async старт и обработка на заявки (ReadRequestAsync, WriteResponseAsync, ProcessClientAsync, AddSession)

- **ServerWeb.Demo** (Console App, net9.0) – демо приложение  
  - `Program.cs` – конфигуриране на маршрути (ConfigureRoutes), pre-render actions (form, cookies, session, login/logout/profile), download helpers, async Main

## Как да стартирате

От папката на решението:

```bash
dotnet run --project ServerWeb.Demo
```

Сървърът слуша на **http://127.0.0.1:8080**.

За друг порт/адрес променете в `Program.cs`, например:

```csharp
var server = new HttpServer(8085, ConfigureRoutes);
// или
var server = new HttpServer(IPAddress.Loopback, 8085, ConfigureRoutes);
```

## Маршрути за тест

| Метод | URL         | Описание |
|-------|-------------|----------|
| GET   | `/`         | Начална страница (HTML) |
| GET   | `/Text`     | Текстови отговор |
| GET   | `/HTML`     | Форма (Name, Age) |
| POST  | `/HTML`     | Показва изпратените form данни |
| GET   | `/Redirect`  | Пренасочване към https://softuni.org/ |
| GET   | `/Content`   | Форма с бутон за download |
| POST  | `/Content`   | Сваляне на content.txt със съдържание от зададените сайтове |
| GET   | `/Cookies`   | Демо с бисквитки (добавяне / показване) |
| GET   | `/Session`   | Информация за сесията (дата на създаване) |
| GET   | `/Login`     | Форма за вход |
| POST  | `/Login`     | Вход (user / user123) |
| GET   | `/Logout`    | Изход |
| GET   | `/UserProfile` | Профил (само за логнати) или насока към Login |

## Зависимости

- **.NET 9.0** (TargetFramework в двата проекта). За .NET 6/7/8 сменете `TargetFramework` в `.csproj` файловете.

## Тест на функционалността

1. **Форми**: GET `/HTML` → попълни Name/Age → Submit → POST показва данните.  
2. **Redirect**: GET `/Redirect` → пренасочване към softuni.org.  
3. **Download**: GET `/Content` → „Download Sites Content“ → POST връща `content.txt`.  
4. **Cookies**: GET `/Cookies` → първо показва „Cookies set!“, при refresh – списък с cookies.  
5. **Session**: GET `/Session` → показва дата на създаване на сесията.  
6. **Login**: GET `/Login` → user / user123 → успех; GET `/UserProfile` → „Welcome, user“; GET `/Logout` → изход; отново `/UserProfile` → насока към Login.
