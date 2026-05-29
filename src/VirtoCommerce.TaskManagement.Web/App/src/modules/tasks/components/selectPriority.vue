<template>
  <TaskPriority :task-priority="item" />
</template>

<script setup lang="ts">
import { TaskPriority as TaskPriorityEnum } from "../../../api_client/virtocommerce.taskmanagement";
import TaskPriority from "./taskPriority.vue";

defineProps<{
  item: TaskPriorityEnum;
}>();
</script>
