"use strict";

class ChatWindow {

    constructor(containerId, fromUserId) {
        this.toUserId = "Admin";
        this.fromUserId = fromUserId;
        this.IsAdmin = fromUserId == "Admin";
        this.LimitMessages = 50;

        this.connection = new signalR.HubConnectionBuilder().withUrl("/notify?userId=" + fromUserId).build();

        this.container = document.getElementById(containerId);
        this.messages = this.container.querySelector('.messages');
        this.disconnectedMessage = this.container.querySelector('.disconnected-message');
        this.headText = this.container.querySelector('.head-text');
        this.inputText = this.container.querySelector('.message-input');
        this.sendButton = this.container.querySelector('.send-btn');
        this.closeButton = this.container.querySelector('.close-btn');
        this.recomendations = this.container.querySelector('.recommended-articles');
        this.loadingIndicator = this.container.querySelector('#loading-indicator');

        if (this.inputText) {
            this.inputText.addEventListener('keydown', async (e) => {
                if (e.keyCode === 13 && !e.shiftKey) {
                    e.preventDefault();
                    await this.SendMessage();
                }
            });
        }

        if (this.closeButton) {
            this.closeButton.addEventListener('click', () => {
                this.setCookie("NopChatActive", "false", 1);
                this.container.style.display = 'none';
            });
        }

        if (this.sendButton) {
            // Agregar un evento 'click' al botón de enviar
            this.sendButton.addEventListener('click', async (e) => {
                e.preventDefault();
                await this.SendMessage();
            });
        }

        this.connection.on("ChatMessages", (messages) => {
            console.log("Receive chat messages");
            messages.reverse();
            for (var i = 0; i < messages.length; i++) {
                var obj = messages[i];
                this.AddMessage(obj.message, obj.fromUserId == this.fromUserId);
            }
        });

        this.connection.on("Message", (message, senderId) => {
            console.log("Receive message from: " + senderId);
            if (this.fromUserId == "Admin" && this.toUserId != senderId)
                return;

            this.AddMessage(message, false);
            this.EnableInputText();
            this.DisableLoading()();
        });

        this.AddMessage("¡Hola! 👋 Bienvenido a nuestro asistente virtual de compras, impulsado por el motor de chat GPT. Estoy aquí para ayudarte a encontrar los productos perfectos para ti. Ya sea que busques algo nuevo, tengas preguntas o necesites recomendaciones, estoy listo para asistirte. ¡Explora con confianza y encuentra justo lo que necesitas! 😄", false);
    }

    ShowLoading() {
        this.loadingIndicator.style.display = 'block';

        const textos = ["Procesando solicitud...", "Accediendo a DB...", "Analizando datos...",  "Generando respuesta...", "Optimizando respuesta..."];
        let indice = 0;

        // Usamos una función de flecha para mantener el contexto de `this`
        const cambiarTexto = () => {
            const contenedor = this.loadingIndicator.querySelector("p"); // Cambiado findelement por querySelector
            contenedor.innerText = textos[indice];
            indice = (indice + 1) % textos.length; // Reiniciar al llegar al final del array
        };

        // Guardamos la referencia del intervalo
        this.intervalID = setInterval(cambiarTexto, 3000);
    }

    DisableLoading() {
        this.loadingIndicator.style.display = 'none';

        // Detenemos el intervalo usando clearInterval
        if (this.intervalID) {
            clearInterval(this.intervalID);
            this.intervalID = null; // Limpiamos la referencia
        }
    }
    EnableInputText() {
        this.inputText.disabled = false;
        this.inputText.classList.remove("disabled");
        this.inputText.focus();
    }

    async SendMessage() {
        

        var messageText = this.inputText.value.trim(); // Eliminar espacios en blanco
        if (messageText === "" || this.fromUserId === this.toUserId) {
            return;
        }

        this.ShowLoading();
        await this.TryConnect();

        this.AddMessage(messageText, true);
        this.inputText.value = '';
        this.inputText.disabled = true;
        this.inputText.classList.add("disabled");

        console.log(`SendMessage From: '${this.fromUserId}' To: '${this.toUserId}' - ${messageText}`);

        
        await this.connection.invoke("UserSendMessage", messageText);
    }

    async TryConnect() {
        if (this.connection.state === signalR.HubConnectionState.Disconnected) {
            try {
                await this.connection.start();
                console.log("ChatWindow SignalR Connected.");
            } catch (error) {
                console.error("Connection error:", error);
            }
        }
    }

    AddMessage(message, fromMe) {
        // Verificar si el mensaje contiene <div> para determinar si es HTML
        if (!fromMe && message.includes('<div')) {
            this.recomendations.innerHTML = message;
            this.messages.scrollLeft = 0;
        } else {
            // Crear un nuevo elemento de mensaje
            let obj = document.createElement('div');
            obj.className = fromMe ? "message from-me" : "message from-others";
            obj.innerText = message; // Agregar el mensaje como texto
            // Agregar el mensaje a la lista de mensajes
            this.messages.appendChild(obj);
            // Desplazar hacia abajo automáticamente el área de mensajes
            this.messages.scrollTop = this.messages.scrollHeight;
        }
    }

}
