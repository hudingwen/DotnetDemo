let socket = null;
let eventCallbacks = [];

const connectWebSocket = (url) => {
  if (!socket || socket.readyState !== WebSocket.OPEN) {
    socket = new WebSocket(url);

    socket.onopen = () => {
      console.log("WebSocket 连接成功");
    };

    socket.onmessage = (event) => {
      eventCallbacks.forEach(callback => callback(event.data));
    };

    socket.onclose = () => {
      console.log("WebSocket 连接已关闭");
    };
  }
};

const sendMessage = (message) => {
  if (socket && socket.readyState === WebSocket.OPEN) {
    socket.send(message);
  } else {
    console.log("WebSocket 连接未建立");
  }
};

const addMessageListener = (callback) => {
  eventCallbacks.push(callback);
};

const removeMessageListener = (callback) => {
  eventCallbacks = eventCallbacks.filter(cb => cb !== callback);
};

export { connectWebSocket, sendMessage, addMessageListener, removeMessageListener };
