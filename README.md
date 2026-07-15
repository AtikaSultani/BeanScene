# ☕ BeanScene – Restaurant Reservation Management System

BeanScene is a full-stack restaurant reservation platform I built for my Diploma of Information Technology (Advanced Programming). The idea was to create something a real café or restaurant could actually use: customers can book a table online, staff can manage incoming reservations, and managers get a dashboard to oversee everything without digging through spreadsheets.

It's built on ASP.NET Core MVC, and I used it as a chance to go beyond basic CRUD — adding real-time features with SignalR, dynamic image generation with SkiaSharp, and proper role-based authentication rather than a single generic login.

## What it does

**For customers**
Users can register, confirm their email, and book a table without needing to call or email the restaurant. They can also check the status of their reservation and get in touch through a contact form. The UI is responsive, so it works fine on mobile.

**For staff**
Staff have their own view where they can see incoming reservations, update statuses, and send confirmation emails once a booking is approved.

**For managers**
Managers get an admin dashboard with full control — managing users, overseeing all reservations, and exporting reports to Excel for record-keeping.

**A couple of extras I'm proud of**
- Real-time chat using SignalR, so staff and customers (or staff and managers) can message without refreshing the page
- Dynamic 2D image generation with SkiaSharp — this was one of the more interesting parts of the build
- Automated email notifications via SMTP for confirmations and updates

## Tech stack

**Backend:** ASP.NET Core MVC (.NET 8), C#, Entity Framework Core, ASP.NET Identity, SignalR
**Database:** Microsoft SQL Server
**Frontend:** Razor Views, HTML5, CSS3, Bootstrap 5, JavaScript
**Graphics:** SkiaSharp
**Tools:** Visual Studio 2022, SQL Server Management Studio, Git & GitHub

## Architecture

The app follows a standard MVC structure:

- **Presentation layer** – Razor Views + Bootstrap
- **Business logic layer** – Controllers and Services
- **Data layer** – Entity Framework Core + SQL Server

```text
BeanScene
│
├── Controllers
├── Models
├── Views
├── Data
├── Services
├── Areas (Identity)
├── Hubs (SignalR)
├── wwwroot
└── Program.cs
```

> Note: if your project doesn't actually have a Blazor Components folder, leave it out of the structure above — only include folders that are really in the repo, since anyone cloning it will notice the mismatch.

## Getting started

```bash
git clone https://github.com/AtikaSultani/BeanScene.git
cd BeanScene
```

1. Update the connection string in `appsettings.json` to point to your local SQL Server instance
2. Run the EF Core migrations:
   ```bash
   dotnet ef database update
   ```
3. Run the app:
   ```bash
   dotnet run
   ```
4. Open `https://localhost:5001` (or whichever port it starts on) in your browser

![](image.png)
## Screenshots
Aailable in the screenshots folder

## What I learned

This project pushed me to work with a few things I hadn't touched much before — SignalR for real-time features and SkiaSharp for image generation were both new territory. Beyond the technical stack, it was also a good exercise in structuring a larger MVC app cleanly, handling authentication and role-based access properly, and thinking through the whole flow from a customer booking a table to a manager exporting a report.

## Author

**Atika Sultani**
Software Developer / QA Engineer, based in Sydney, Australia
GitHub: [github.com/AtikaSultani](https://github.com/AtikaSultani)
LinkedIn: *(add your URL here)*

## License

Built for educational purposes as part of my Diploma of Information Technology (Advanced Programming) at TAFE NSW.
