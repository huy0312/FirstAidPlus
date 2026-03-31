// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// AI Chat Offcanvas Logic
window.handleOffcanvasEnter = function (e) {
    if (e.key === 'Enter') sendOffcanvasMessage();
};

window.sendOffcanvasMessage = async function () {
    const input = document.getElementById('offcanvasChatInput');
    const message = input.value.trim();
    if (!message) return;

    // Append User Message
    appendOffcanvasMessage(message, true);
    input.value = '';

    try {
        const response = await fetch('/Chat/GetAIResponse', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ message: message })
        });

        const data = await response.json();
        if (data.reply) {
            appendOffcanvasMessage(data.reply, false, data.timestamp);
        }
    } catch (error) {
        console.error('Chat error:', error);
        appendOffcanvasMessage("Xin lỗi, tôi đang gặp sự cố kết nối.", false);
    }
};

window.appendOffcanvasMessage = function (text, isUser, time = 'Bây giờ') {
    const chatBody = document.getElementById('offcanvasChatBody');
    if (!chatBody) return;

    const div = document.createElement('div');
    div.className = `d-flex mb-3 ${isUser ? 'justify-content-end' : 'justify-content-start'}`;

    div.innerHTML = `
        <div class="${isUser ? 'bg-danger text-white' : 'bg-light text-dark'} p-3 rounded-4 shadow-sm" style="max-width: 85%; font-size: 0.9rem; border-bottom-${isUser ? 'right' : 'left'}-radius: 4px;">
            <div>${text}</div>
            <div class="${isUser ? 'text-white-50' : 'text-muted'} text-end mt-1" style="font-size: 0.65rem;">${time}</div>
        </div>
    `;

    chatBody.appendChild(div);
    chatBody.scrollTop = chatBody.scrollHeight;
};
