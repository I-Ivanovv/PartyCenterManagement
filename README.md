# PartyCenterManagement

PartyCenterManagement е уеб базирана система за управление на парти център, разработена с ASP.NET Core MVC, Entity Framework Core и ASP.NET Identity.

## Описание

Системата позволява управление на резервации, потребители, пакети и допълнителни услуги. Проектът включва различни роли за достъп, като клиентите могат да създават резервации, служителите могат да ги управляват, а администраторите имат пълен контрол върху системата.

## Основни функционалности

- Регистрация и вход на потребители  
- Управление на потребителски роли  
- Създаване на резервации от регистрирани потребители и гости  
- Избор на пакет и допълнителни услуги  
- Автоматично изчисляване на цена  
- Управление на резервации  
- Клиентска страница „My Reservations“  
- Служителски панел за управление на резервации  
- Административен панел  
- Управление на пакети и услуги  
- Управление на потребители  
- Календар за визуализация на резервации  
- Dashboard със статистика  

## Използвани технологии

- ASP.NET Core MVC  
- ASP.NET Identity  
- Entity Framework Core  
- SQL Server  
- Razor Views  
- Bootstrap  
- JavaScript  

## Структура на проекта
PartyCenterManagement/
```
PartyCenterManagement/
│
├── Controllers/
├── Data/
├── Migrations/
├── Models/
│   └── ViewModels/
├── Services/
├── Views/
│   ├── Admin/
│   ├── Employee/
│   ├── Home/
│   ├── Reservation/
│   └── Shared/
├── Areas/
│   └── Identity/
└── wwwroot/
```

## Роли в системата

Системата използва три основни роли:

Admin – пълен достъп до системата (управление на потребители, пакети, услуги и резервации)  
Employee – управление на резервации  
Client – създаване и преглед на собствени резервации  

## Seed-нати потребители

При стартиране на проекта се създават начални потребители:

Email: admin@party.local  
Password: Admin99*  
Role: Admin  

Email: employee@party.local  
Password: Employee99*  
Role: Employee  

Email: user@party.local  
Password: User99*  
Role: Client  

## Стартиране на проекта

## 1. Клониране на репозиторито

Отвори терминал или Command Prompt и изпълни:

```bash
git clone https://github.com/your-username/PartyCenterManagement.git
```

След това влез в папката на проекта:

```bash
cd PartyCenterManagement
```

## 2. Провери connection string-а

Отвори файла:

`appsettings.json`

и провери дали `DefaultConnection` сочи към твоя SQL Server или LocalDB база за проекта **PartyCenterManagement**.

Пример:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PartyCenterManagement;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

## 3. Приложи миграциите

В Package Manager Console или терминал изпълни:

```bash
dotnet ef database update
```

## 4. Стартирай проекта

Изпълни:

```bash
dotnet run
```

## Основни страници

Публични:  
- Home  
- Packages  
- Contact  
- Reservation Calendar  
- Reserve  

Клиент:  
- My Reservations  
- Reserve  

Служител:  
- Manage Reservations  

Админ:  
- Dashboard  
- Packages and Services  
- Users  

## Автор

Проектът е разработен като дипломна работа на тема:  
„Уеб базирана система за управление на резервации в парти център“
