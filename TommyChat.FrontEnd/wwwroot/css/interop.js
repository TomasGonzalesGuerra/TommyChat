/**
 * Hace scroll hasta el fondo del contenedor de mensajes.
 * @param {string} elementId  — id del div .chat-body
 */
window.scrollChatToBottom = function (elementId) {
    const el = document.getElementById(elementId);
    if (el) el.scrollTop = el.scrollHeight;
};
