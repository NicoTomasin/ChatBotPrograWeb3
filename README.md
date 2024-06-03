[![Build And Test](https://github.com/NicoTomasin/ChatBotPrograWeb3/actions/workflows/BuildAndTest.yml/badge.svg)](https://github.com/NicoTomasin/ChatBotPrograWeb3/actions/workflows/BuildAndTest.yml)
# Bot Recomendador de peliculas

La idea detras del bot es que mediante diferentes menus, te recomiende una pelicula en base a tu estado de animo SIN el uso de NLPs



## API The Movie DB

#### Get By Genre

```http
  GET api.themoviedb.org/3/genre/movie/list
```

| Parámetro        | Tipo     | Descripción                                                                                               |
| :--------------- | :------- | :-------------------------------------------------------------------------------------------------------- |
| `api_key`        | `string` | **Requerido**. Clave de API                                                                               |

#### Get Movie

```http
  GET api.themoviedb.org/3/discover/movie
```

| Parámetro        | Tipo     | Descripción                                                                                               |
| :--------------- | :------- | :-------------------------------------------------------------------------------------------------------- |
| `api_key`        | `string` | **Requerido**. Clave de API                                                                               |
| `with_genres`    | `string` | *Opcional*. Género                                                                                        |
| `sort_by`        | `string` | *Opcional*. original_title, popularity, revenue, primary_release_date, title, vote_average, vote_count (Se agrega `asc` o `desc` al final, por ejemplo: `vote_average.desc`) |
| `vote_count.gte` | `float`  | *Opcional*. 10000                                                                                        |

#### Get Poster

```http
  GET image.tmdb.org/t/p/w300_and_h450_bestv2

```

| Parametro | Tipo     | Descripcion                |
| :-------- | :------- | :------------------------- |
| `api_key` | `string` | **Required**. API key |
| `poster_path` | `string` | *Opcional*. |


## Documentacion

 - [Drive](https://docs.google.com/document/d/1XGZJ6KxPlzzKlTp20Y8wM7EDfWklLWJz)


## Authores
- [@lucamodic](https://github.com/lucamodic)
- [@NicoTomasin](https://github.com/NicoTomasin)


![Logo](https://miel.unlam.edu.ar/vista/imagenes/logos/unlam/logo-unlam-light-40.png)

