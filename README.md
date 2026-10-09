# ServiceEvents

Веб-приложение для планирования небольших мероприятий внутри подразделения: сотрудники просматривают события и записываются на них, организаторы управляют своими мероприятиями, администратор настраивает пользователей, департаменты и характеристики событий.

## Возможности

- Авторизация пользователей с ролями администратора, организатора и сотрудника.
- Каталог опубликованных мероприятий с описанием, временем, местом, изображением и ограничением числа участников.
- Запись на мероприятие и отмена записи.
- Создание, редактирование, публикация и управление мероприятиями.
- Настраиваемые характеристики мероприятий: текст, число и значение «да/нет».
- Управление департаментами; пользователь относится к одному департаменту, а новое мероприятие получает департамент своего организатора.
- Каталог сотрудника показывает мероприятия его департамента.

Изображение указывается ссылкой HTTPS или путём к статическому файлу сайта, например `/images/event.jpg`. Загрузка файла изображения через форму не реализована.

## Технологии

- .NET 8 / ASP.NET Core MVC
- Entity Framework Core 8
- PostgreSQL и Npgsql
- Bootstrap

## Требования

- .NET 8 SDK
- PostgreSQL
- Инструмент Entity Framework Core CLI версии 8:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.31
```

Если `dotnet-ef` уже установлен, обновите его до совместимой версии при необходимости:

```powershell
dotnet tool update --global dotnet-ef --version 8.0.31
```

## Запуск локально

1. Клонируйте репозиторий и перейдите в его корень.
2. Создайте базу PostgreSQL, например `ServiceEvents`.
3. Настройте строку подключения.

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=ServiceEvents;Username=postgres;Password=YOUR_PASSWORD"
```

Либо укажите строку подключения локально в `ServiceEvents.Web\appsettings.json` или `ServiceEvents.Web\appsettings.Development.json`.

4. Восстановите зависимости и примените миграции:

```powershell
dotnet restore ServiceEvents.slnx
dotnet ef database update --project ServiceEvents.Infrastructure\ServiceEvents.Infrastructure.csproj --startup-project ServiceEvents.Web\ServiceEvents.Web.csproj
```

5. Запустите сайт:

```powershell
dotnet run --project ServiceEvents.Web\ServiceEvents.Web.csproj
```

Профили запуска Visual Studio используют окружение `Development`; для локального профиля HTTP адрес по умолчанию — `http://localhost:5282`, HTTPS — `https://localhost:7214`.

## Тестовые учётные записи

При запуске в `Development` начальные данные автоматически создаются или дополняются: департамент «Информационные технологии», тестовые пользователи, характеристики и несколько опубликованных мероприятий.

| Роль | Email | Пароль |
| --- | --- | --- |
| Администратор | `admin@gmail.com` | `Admin123!` |
| Организатор | `organizer@localhost` | `Organizer123!` |
| Сотрудник | `employee@localhost` | `Employee123!` |

## Сборка

```powershell
dotnet build ServiceEvents.slnx
```

## Структура решения

- `ServiceEvents.Web` — MVC-интерфейс, области ролей, контроллеры и представления.
- `ServiceEvents.ApplicationServiceEvents.Application` — бизнес-сервисы, интерфейсы и DTO.
- `ServiceEvents.Domain` — сущности и доменные перечисления.
- `ServiceEvents.Infrastructure` — PostgreSQL, EF Core, репозитории и миграции.
