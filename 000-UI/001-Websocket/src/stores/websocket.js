import { defineStore } from "pinia";

export const useWebSocketStore = defineStore("websocket", {
  state: () => ({
    socket: null,
    messages: [],
  }),
  actions: {
    connect(url) {
      if (!this.socket || this.socket.readyState !== WebSocket.OPEN) {
        this.socket = new WebSocket(url);
        this.socket.onopen = () => console.log("WebSocket 连接成功");
        this.socket.onmessage = (event) => this.messages.push(event.data);
        this.socket.onclose = () => console.log("WebSocket 连接关闭");
      }
    },
    sendMessage(message) {
      if (this.socket && this.socket.readyState === WebSocket.OPEN) {
        this.socket.send(message);
      } else {
        console.log("WebSocket 未连接");
      }
    },
  },
});
