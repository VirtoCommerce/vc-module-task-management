<template>
  <VcStatus v-bind="statusStyles[workTaskStatus]">
    {{ localizedStatus() }}
  </VcStatus>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";

export interface Props {
  workTaskStatus: string;
}

defineOptions({
  inheritAttrs: false,
});

const { t } = useI18n({ useScope: "global" });

const props = withDefaults(defineProps<Props>(), {
  workTaskStatus: "ToDo",
});

const localizedStatus = () => {
  let result = t("TASKS.STATUS.TODO");

  switch (props.workTaskStatus) {
    case "ToDo":
      result = t("TASKS.STATUS.TODO");
      break;
    case "Canceled":
      result = t("TASKS.STATUS.CANCELED");
      break;
    case "Done":
      result = t("TASKS.STATUS.DONE");
      break;
  }
  return result;
};

const statusStyles = {
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
