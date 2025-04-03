<script setup>
import { ref, onMounted, onUnmounted } from "vue";
import { connectWebSocket, sendMessage, addMessageListener, removeMessageListener } from "@/utils/websocket";

const message = ref("");
const receivedMessage = ref("");

const handleMessage = (data) => {
  receivedMessage.value = data;
};

onMounted(() => {
  connectWebSocket("ws://localhost:5037/ws?token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJkYXRhIjpbeyJ0b29sdHQiOiJodHRwczovL3Rvb2x0dC5jb20ifV0sImlhdCI6MTc0MjI4NzY3OSwiZXhwIjoxOTYzMTUxOTk5LCJhdWQiOiIiLCJpc3MiOiIiLCJzdWIiOiJodWRpbmd3ZW4ifQ.E9T5CBT0PSs-Vy37HxgHj6hx-K9_Uwi0RmafCGOuOO8");
  addMessageListener(handleMessage);
});

onUnmounted(() => {
  removeMessageListener(handleMessage);
});

const sendMsg = () => {
  sendMessage(message.value);
};
</script>

<template>
  <div>
    <h2>WebSocket 交互</h2>
    <input v-model="message" placeholder="输入消息" />
    <button @click="sendMsg">发送</button>
    <p>服务器返回：{{ receivedMessage }}</p>
  </div>
</template>
