function mostrarTrailer(movieId) {
    $.ajax({
        type: 'GET',
        url: '/Home/GetTrailerUrl',
        data: { movieId: movieId },
        success: function (url) {
            window.open(url, '_blank');
        },
        error: function () {
            alert('Error al cargar el tráiler.');
        }
    });
}


    $(document).ready(function () {
        // Inicializar el carrusel
        $('#myCarousel').carousel({
            interval: 5000
        });

    // Avanzar al siguiente slide cada cierto tiempo
    setInterval(function () {
        $('#myCarousel').carousel('next');
            }, 5000); 
        });
