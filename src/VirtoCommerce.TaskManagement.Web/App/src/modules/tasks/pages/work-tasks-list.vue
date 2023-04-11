<template>
  <TasksList
    :closable="props.closable"
    :expanded="props.expanded"
    :param="props.param"
    :title="$t('TASKS.PAGES.LIST.ALL_TASKS_TITLE')"
    :isCurrentUserList="false"
    :onlyComplitedList="false"
    @onItemClick="onItemClick"
    @newTaskClick="newTaskClick"
    ref="tasksList"
  ></TasksList>
</template>

<script lang="ts">
import { IBladeEvent } from "@vc-shell/framework";
import { defineComponent, ref, shallowRef } from "vue";
import { WorkTaskDetails } from ".";
import TasksList from "../components/tasksList.vue";

export default defineComponent({
  url: "/active",
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

const emit = defineEmits<Emits>();

const tasksList = ref(null);
const onItemClick = (item: { id: string }) => {
  emit("open:blade", {
    component: shallowRef(WorkTaskDetails),
    param: item.id,
  });
};

const newTaskClick = () => {
  emit("open:blade", {
    component: shallowRef(WorkTaskDetails),
  });
};

const reload = async () => {
  await tasksList.value.reload();
};

defineExpose({
  reload,
  onItemClick,
});
</script>
