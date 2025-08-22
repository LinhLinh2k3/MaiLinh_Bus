$(document).ready(function () {
    // Sự kiện click để hiển thị/ẩn chatbox
    $('#chat-icon').on('click', function () {
        $('#chat-container').toggle();
    });

    $('#close-chat-btn').on('click', function () {
        $('#chat-container').hide();
    });

    function toggleChat() {
        const popup = document.getElementById("chatPopup");
        popup.style.display = popup.style.display === "flex" ? "none" : "flex";
    }

    // Hàm thêm tin nhắn vào giao diện
    function appendMessage(sender, message) {
        const messageClass = sender === 'user' ? 'user-message' : 'bot-message';
        const messageHtml = `<div class="message ${messageClass}">${message}</div>`;
        $('#chat-messages').append(messageHtml);
        $('#chat-messages').scrollTop($('#chat-messages')[0].scrollHeight);
    }

    // Hàm hiển thị trạng thái đang xử lý
    function showLoading() {
        const loadingHtml = `<div id="loading-indicator" class="loading"><span class="bg-primary"></span><span class="bg-primary"></span><span class="bg-primary"></span></div>`;
        $('#chat-messages').append(loadingHtml);
        $('#chat-messages').scrollTop($('#chat-messages')[0].scrollHeight);
    }

    // Hàm ẩn trạng thái đang xử lý
    function hideLoading() {
        $('#loading-indicator').remove();
    }

    // Hàm gửi tin nhắn đến API back-end C#
    async function sendMessage() {
        const userMessage = $('#chat-input').val().trim();
        if (userMessage === '') return;

        appendMessage('user', userMessage);
        $('#chat-input').val('');
        showLoading();

        try {
            const response = await fetch('/api/chat', { // Endpoint API của C#
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ message: userMessage })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            hideLoading();
            appendMessage('bot', data.response.replace(/\n/g, '<br>'));

        } catch (error) {
            hideLoading();
            console.error("Lỗi khi gọi API:", error);
            appendMessage('bot', 'Xin lỗi, tôi không thể kết nối tới dịch vụ. Vui lòng thử lại sau.');
        }
    }

    // Gán sự kiện click và keypress
    $('#send-btn').on('click', sendMessage);
    $('#chat-input').on('keypress', function (e) {
        if (e.which === 13) {
            sendMessage();
        }
    });
});