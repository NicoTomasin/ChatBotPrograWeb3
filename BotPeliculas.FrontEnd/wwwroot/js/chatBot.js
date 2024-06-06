document.getElementById('chatButton').addEventListener('click', function () {
    var chatWindow = document.getElementById('chatWindow');
    if (chatWindow.style.display === 'none' || chatWindow.style.display === '') {
        chatWindow.style.display = 'block';
    } else {
        chatWindow.style.display = 'none';
    }
});


/*Scroll */
store.subscribe(() => {
    const state = store.getState();
    const chatBody = document.getElementById('chatBody');
    if (state.activities.length > 0) {
        setTimeout(() => {
            chatBody.scrollTop = chatBody.scrollHeight;
        }, 100);
    }
});