<template>
  <VcIcon v-bind:icon="priorityIcon()" :class="getClass()"></VcIcon>
  <span>{{ showText() }}</span>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  inheritAttrs: false,
});
</script>

<script lang="ts" setup>
import { VcIcon } from "@vc-shell/framework";
import { WorkTaskPriority } from "../../../api_client/taskmanagement";

export interface Props {
  workTaskPriority: WorkTaskPriority;
  withText: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  workTaskPriority: undefined,
  withText: false,
});

const priorityIcon = () => {
  let result = "fas fa-equals";

  switch (props.workTaskPriority) {
    case WorkTaskPriority.Lowest:
      result = "fas fa-chevron-down";
      break;
    case WorkTaskPriority.Low:
      result = "fas fa-chevron-down";
      break;
    case WorkTaskPriority.Normal:
      result = "fas fa-equals";
      break;
    case WorkTaskPriority.High:
      result = "fas fa-chevron-up";
      break;
    case WorkTaskPriority.Highest:
      result = "fas fa-chevron-up";
      break;
    default:
      result = "fas fa-equals";
      break;
  }

  return result;
};

const showText = () => {
  return props.withText ? " " + props.workTaskPriority : "";
};

const getClass = () => {
  return props.workTaskPriority?.toString().toLowerCase();
};
</script>

<style lang="scss">
.lowest {
  color: #9fdb31;
}
.low {
  color: yellowgreen;
}
.normal {
  color: #d1b717;
}
.high {
  color: orange;
}
.highest {
  color: orangered;
}
</style>
