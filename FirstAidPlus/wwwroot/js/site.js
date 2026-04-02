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

    // Show Typing Indicator
    const typingId = showTypingIndicator();

    try {
        const response = await fetch('/Chat/GetAIResponse', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ message: message })
        });

        const data = await response.json();
        
        // Hide Typing Indicator
        hideTypingIndicator(typingId);

        if (data.reply) {
            appendOffcanvasMessage(data.reply, false, data.timestamp);
            // Play a subtle notification sound if desired
        }
    } catch (error) {
        console.error('Chat error:', error);
        hideTypingIndicator(typingId);
        appendOffcanvasMessage("Xin lỗi, tôi đang gặp sự cố kết nối. Hãy thử lại sau nhé!", false);
    }
};

window.showTypingIndicator = function () {
    const chatBody = document.getElementById('offcanvasChatBody');
    const id = 'typing-' + Date.now();
    const div = document.createElement('div');
    div.id = id;
    div.className = 'd-flex mb-3 justify-content-start animate__animated animate__fadeIn';
    div.innerHTML = `
        <div class="chat-bubble chat-bubble-ai px-3 py-2">
            <div class="typing-indicator">
                <span></span><span></span><span></span>
            </div>
        </div>
    `;
    chatBody.appendChild(div);
    chatBody.scrollTop = chatBody.scrollHeight;
    return id;
};

window.hideTypingIndicator = function (id) {
    const el = document.getElementById(id);
    if (el) el.remove();
};

window.appendOffcanvasMessage = function (text, isUser, time = 'Vừa xong') {
    const chatBody = document.getElementById('offcanvasChatBody');
    if (!chatBody) return;

    const div = document.createElement('div');
    div.className = `d-flex mb-3 ${isUser ? 'justify-content-end' : 'justify-content-start'} animate__animated animate__fadeInUp`;

    div.innerHTML = `
        <div class="chat-bubble ${isUser ? 'chat-bubble-user' : 'chat-bubble-ai'}">
            <div style="white-space: pre-wrap;">${text}</div>
            <div class="${isUser ? 'text-white-50' : 'text-muted'} text-end mt-2" style="font-size: 0.65rem;">
                <i class="far fa-clock me-1"></i>${time}
            </div>
        </div>
    `;

    chatBody.appendChild(div);
    chatBody.scrollTo({ top: chatBody.scrollHeight, behavior: 'smooth' });
};
