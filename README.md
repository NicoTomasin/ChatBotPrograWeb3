<div align="center">

# 🎬 Bot Recomendador de Películas

**Chatbot que te recomienda películas según tu estado de ánimo, guiado por menús y sin usar NLP.**

[![Build And Tests](https://github.com/NicoTomasin/ChatBotPrograWeb3/actions/workflows/BuildAndTest.yml/badge.svg)](https://github.com/NicoTomasin/ChatBotPrograWeb3/actions/workflows/BuildAndTest.yml)
![.NET 6](https://img.shields.io/badge/.NET_6-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white)
![Bot Framework](https://img.shields.io/badge/Bot_Framework-0078D4?style=flat-square&logo=microsoft&logoColor=white)
![TMDB](https://img.shields.io/badge/TMDB_API-01B4E4?style=flat-square&logo=themoviedatabase&logoColor=white)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)

</div>

---

## 📖 Sobre el proyecto

Trabajo práctico de **Programación Web 3** (Tecnicatura en Desarrollo Web, UNLaM). En lugar de interpretar lenguaje natural, el bot conversa a través de **tarjetas con botones**: elegís una opción, el bot consulta [The Movie Database (TMDB)](https://www.themoviedb.org/) y te responde con el póster, la info de la película y su tráiler.

El proyecto incluye además un **front-end web** con registro e inicio de sesión de usuarios, donde se embebe el chat con Bot Framework Web Chat.

## ✨ Funcionalidades

**Menú principal**

| Opción | Qué hace |
| --- | --- |
| 🔥 Ver películas del día | Top 10 de películas en tendencia |
| 🎯 Recomendar película | Te pregunta cómo te sentís y te sugiere películas acordes |
| 📅 Próximas películas | Estrenos que están por llegar |

**Recomendación por estado de ánimo**

| Estado | Género TMDB |
| --- | --- |
| 😢 Triste | Drama |
| 😄 Feliz | Comedia |
| 😠 Enojado | Acción |
| ❤️ Romántico | Romance |
| 😰 Nervioso | Terror |
| 🤔 Curioso | Documental |

Para cada estado podés pedir el **Top 10 más visto**, **la más vista** o una **película random**.

## 🏗️ Arquitectura

```
ChatBotPrograWeb3/
├── BotPeliculas.Bot/          # Bot (ASP.NET Core + Bot Framework SDK v4), diálogos en Dialogs/
├── BotPeliculas.FrontEnd/     # Web MVC: registro/login de usuarios + Web Chat embebido
├── BotPeliculas.Services/     # Lógica de negocio y consumo de la API de TMDB
├── BotPeliculas.Interfaces/   # Contratos (IPeliculasService, ...)
├── BotPeliculas.Models/       # Modelos, URLs de TMDB y DbContext de EF Core + migraciones
├── BotPeliculas.Data/         # Contexto de datos (EF Core)
├── BotPelicualas.Logica/      # Servicio de usuarios
└── BotPeliculas.Tests/        # Tests unitarios con NUnit + Moq
```

## 🧰 Stack

- **.NET 6** / **ASP.NET Core** (bot y MVC)
- **Microsoft Bot Framework SDK v4** (Dialogs, Hero Cards, Adaptive Cards)
- **Entity Framework Core 6** + **SQL Server**
- **NUnit** + **Moq** para tests
- **GitHub Actions** para build y tests en cada push

## 🚀 Cómo correrlo

### Requisitos

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- SQL Server (o SQL Server Express) para el front-end
- [Bot Framework Emulator](https://github.com/microsoft/BotFramework-Emulator) para probar el bot
- Una API key de [TMDB](https://www.themoviedb.org/settings/api)

### Pasos

```bash
git clone https://github.com/NicoTomasin/ChatBotPrograWeb3.git
cd ChatBotPrograWeb3

# Compilar toda la solución
dotnet build BotPeliculas.Bot/BotPeliculas.sln

# Levantar el bot (escucha en http://localhost:3978)
dotnet run --project BotPeliculas.Bot
```

Después, abrí el **Bot Framework Emulator** y conectate a `http://localhost:3978/api/messages`.

Para el front-end web:

1. Ajustá la cadena de conexión en `BotPeliculas.Models/EF/BotPeliculasContext.cs` a tu instancia de SQL Server.
2. Aplicá las migraciones: `dotnet ef database update --project BotPeliculas.Models`
3. Ejecutá `dotnet run --project BotPeliculas.FrontEnd`.

### Tests

```bash
dotnet test BotPeliculas.Tests/BotPeliculas.Tests.csproj
```

## 🔌 API de TMDB utilizada

| Endpoint | Uso |
| --- | --- |
| `GET /3/trending/movie/day` | Películas del día |
| `GET /3/discover/movie?with_genres={id}&sort_by=popularity.desc` | Recomendación por género |
| `GET /3/movie/upcoming` | Próximos estrenos |
| `GET /3/movie/{id}/videos` | Tráiler |
| `GET image.tmdb.org/t/p/w300_and_h450_bestv2/{poster_path}` | Póster |

Todas requieren el parámetro `api_key`.

## 👥 Equipo

- [Nico Tomasin](https://github.com/NicoTomasin)
- [Gonzalo Saldaño](https://github.com/GonzaSalda)
- [Luca Modic](https://github.com/lucamodic)
- [Lucas Gomez Villa](https://github.com/LucasGomezz)

📄 Documentación del TP: [Google Drive](https://docs.google.com/document/d/1XGZJ6KxPlzzKlTp20Y8wM7EDfWklLWJz)

## 🤝 Contribuir

Leé la [guía de contribución](CONTRIBUTING.md) antes de abrir un PR.

## 📄 Licencia

Distribuido bajo la [Licencia MIT](LICENSE). Este producto usa la API de TMDB pero no está avalado ni certificado por TMDB.
