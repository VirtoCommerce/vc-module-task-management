<template>
  <TasksList
    :expanded="props.expanded"
    :param="props.param"
    :title="$t('TASKS.PAGES.LIST.ARCHIVE_TASKS_TITLE')"
    :isCurrentUserList="false"
    :onlyComplitedList="true"
    ref="tasksList"
  ></TasksList>
</template>

<script lang="ts">
import { IBladeEvent } from "@vc-shell/framework";
import { defineComponent, ref } from "vue";
import TasksList from "../components/tasksList.vue";

export default defineComponent({
  url: "/archive",
});
</script>

<script lang="ts" setup>
export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}

export interface Emits {
  (event: "close:blade"): void;
  (event: "open:blade", blade: IBladeEvent): void;
}

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
  param: undefined,
});

const tasksList = ref(null);

const reload = async () => {
  await tasksList.value.reload();
};

defineExpose({
  reload,
});
</script>
