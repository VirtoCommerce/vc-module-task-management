<template>
  <div>
    <VcStatus
      v-bind="statusStyles[itemStatus]"
      :class="$attrs.class"
      >{{ $t(`TASKS.STATUS.${itemStatus.toUpperCase()}`) }}</VcStatus
    >
  </div>
</template>

<script lang="ts" setup>
import { computed, toRefs } from "vue";
import { IWorkTask } from "../../../api_client/virtocommerce.taskmanagement";

export interface Props {
  context: {
    item: IWorkTask;
  };
}

const props = withDefaults(defineProps<Props>(), {
  context: () => ({
    item: {
      status: "ToDo",
    },
  }),
});

const { context } = toRefs(props);
const itemStatus = computed(() => getStatus(context.value.item) || "ToDo");

function getStatus(task: IWorkTask) {
  if (task.isActive) {
    return "ToDo";
  }

  if (task.completed) {
    return "Done";
  }

  return "Canceled";
}

const statusStyles: Omit<Record<string, Record<string, unknown>>, "ToDo"> = {
  ToDo: {
    outline: false,
    variant: "info",
  },
  Canceled: {
    outline: false,
    variant: "danger",
  },
  Done: {
    outline: false,
    variant: "success",
  },
};
</script>
