<template>
  <div class="tw-flex tw-flex-row tw-items-center">
    <VcIcon
      icon="material-circle"
      :class="{
        'tw-text-[10px]': true,
        'tw-text-[#80b4e3]': [WorkTaskPriority.Low, WorkTaskPriority.Lowest].indexOf(taskPriority!) > -1,
        'tw-text-[#87b563]': taskPriority === WorkTaskPriority.Normal,
        'tw-text-[#eb4f4d]': [WorkTaskPriority.High, WorkTaskPriority.Highest].indexOf(taskPriority!) > -1,
      }"
    ></VcIcon>
    <span class="tw-ml-1">{{ taskPriority }}</span>
  </div>
</template>

<script setup lang="ts">
import { WorkTaskPriority } from "../../../api_client/virtocommerce.taskmanagement";

defineProps<{
  taskPriority: WorkTaskPriority;
  slotName?: string;
}>();
</script>
