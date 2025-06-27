<template>
  <VcBlade
    v-loading="loading"
    :title="bladeTitle"
    :width="bladeWidth"
    :expanded="expanded"
    :closable="closable"
    :toolbar-items="toolbarItems"
    :modified="isModified"
    @close="$emit('close:blade')"
    @expand="$emit('expand:blade')"
    @collapse="$emit('collapse:blade')"
  >
    <VcContainer>
      <VcForm class="tw-flex tw-flex-col tw-gap-4">
        <!-- Task Type -->
        <VcSelect
          v-model="item.type"
          :label="$t('TASKS.PAGES.DETAILS.FIELDS.TYPE.TITLE')"
          :options="loadTaskTypes"
          option-value="name"
          option-label="name"
          :disabled="isReadOnly"
        />

        <!-- Task Name (only for new) -->
        <VcInput
          v-model="item.name"
          :label="$t('TASKS.PAGES.DETAILS.FIELDS.SUMMARY.TITLE')"
          :disabled="isReadOnly"
        />

        <!-- Description -->
        <VcEditor
          v-model="item.description"
          :label="$t('TASKS.PAGES.DETAILS.FIELDS.DESCRIPTION.TITLE')"
          :disabled="isReadOnly"
          assets-folder="/assets"
        />

        <!-- Priority -->
         <!-- @vue-generic {WorkTaskPriority} -->
        <VcSelect
          v-model="item.priority"
          :label="$t('TASKS.PAGES.DETAILS.FIELDS.PRIORITY.TITLE')"
          :options="priorities"
          option-value="value"
          option-label="name"
          :disabled="isReadOnly"
        >
          <template #selected-item="{ opt }">
            <SelectPriority :item="opt.value" />
          </template>
          <template #option="{ opt }">
            <SelectPriority :item="opt.value" />
          </template>
        </VcSelect>

        <!-- Due Date -->
        <VcInput
          v-model="item.dueDate"
          type="date"
          :label="$t('TASKS.PAGES.DETAILS.FIELDS.DUE_DATE.TITLE')"
          :disabled="isReadOnly"
        />

        <!-- Responsible -->
        <Field
          v-slot="{ errorMessage, handleChange, errors }"
          name="responsibleId"
          :model-value="item.responsibleId"
          :label="$t('TASKS.PAGES.DETAILS.FIELDS.RESPONSIBLE.TITLE')"
          rules="required"
        >
          <VcSelect
            v-model="item.responsibleId"
            :label="$t('TASKS.PAGES.DETAILS.FIELDS.RESPONSIBLE.TITLE')"
            :options="searchContacts"
            option-value="id"
            option-label="name"
            searchable
            required
            :disabled="isReadOnly"
            :error="errors.length > 0"
            :error-message="errorMessage"
            @update:model-value="handleChange"
          />
        </Field>

        <!-- Status -->
        <VcStatus>{{ statusText }}</VcStatus>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, onBeforeUnmount, inject } from "vue";
import { useI18n } from "vue-i18n";
import { Field, useForm } from "vee-validate";
import {
  BladeInstance,
  IBladeToolbar,
  IParentCallArgs,
  useBladeNavigation,
  usePermissions,
  usePopup,
  useWidgets,
} from "@vc-shell/framework";

import { useWorkTaskDetails } from "../composables/useWorkTask";
import { TaskPermissions } from "../../../types";
import SelectPriority from "../components/selectPriority.vue";
import AssetsWidget from "../components/widgets/assets/assets-widget.vue";
import { WorkTask, WorkTaskPriority } from "../../../api_client/virtocommerce.taskmanagement";

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
  options?: unknown;
}

export interface Emits {
  (event: "parent:call", args: IParentCallArgs): void;
  (event: "close:blade"): void;
  (event: "expand:blade"): void;
  (event: "collapse:blade"): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

defineOptions({
  name: "WorkTaskDetails",
  url: "/work-task-details",
});

const { meta } = useForm({ validateOnMount: false });
const { hasAccess } = usePermissions();
const { registerWidget, unregisterWidget } = useWidgets();
const { onBeforeClose } = useBladeNavigation();
const { showConfirmation } = usePopup();

const blade = inject(BladeInstance);
const { t } = useI18n({ useScope: "global" });

const {
  item,
  isModified,
  loading,
  saveWorkTask,
  completeWorkTask,
  rejectWorkTask,
  deleteWorkTask,
  resetWorkTask,
  isReadOnly,
  priorities,
  loadTaskTypes,
  searchContacts,
  statusText,
  loadWorkTask,
} = useWorkTaskDetails({
  id: props.param,
  isNew: !props.param,
});

// Local state
const bladeWidth = ref(550);

// Blade title
const bladeTitle = computed(() => {
  return props.param ? `# ${item.value?.number}: ${item.value?.name}` : t("TASKS.PAGES.DETAILS.NEW_TITLE");
});

// Reactive toolbar
const toolbarItems = computed((): IBladeToolbar[] => [
  {
    id: "complete",
    icon: "material-check",
    title: t("TASKS.PAGES.DETAILS.TOOLBAR.COMPLETE"),
    isVisible: !!props.param && item.value?.isActive && hasAccess(TaskPermissions.Finish),
    clickHandler: async () => {
      try {
        await completeWorkTask();
        emit("parent:call", { method: "reload" });
      } catch (error) {
        console.error("Failed to complete task:", error);
      }
    },
  },
  {
    id: "reject",
    icon: "material-block",
    title: t("TASKS.PAGES.DETAILS.TOOLBAR.REJECT"),
    isVisible: !!props.param && item.value?.isActive && hasAccess(TaskPermissions.Finish),
    clickHandler: async () => {
      try {
        await rejectWorkTask();
        emit("parent:call", { method: "reload" });
      } catch (error) {
        console.error("Failed to reject task:", error);
      }
    },
  },
  {
    id: "reset",
    icon: "material-undo",
    title: t("TASKS.PAGES.DETAILS.TOOLBAR.RESET"),
    disabled: !isModified.value,
    isVisible: !!props.param && item.value?.isActive && hasAccess(TaskPermissions.Update),
    clickHandler: () => {
      resetWorkTask();
    },
  },
  {
    id: "save",
    icon: "material-save",
    title: t("TASKS.PAGES.DETAILS.TOOLBAR.SAVE"),
    disabled: !isModified.value || !meta.value.valid,
    isVisible:
      (props.param && item.value?.isActive && hasAccess(TaskPermissions.Update)) ||
      (!props.param && item.value?.isActive && hasAccess(TaskPermissions.Create)),
    clickHandler: async () => {
      try {
        await saveWorkTask();
        emit("parent:call", { method: "reload" });
        emit("parent:call", { method: "onItemClick", args: item.value });
      } catch (error) {
        console.error("Failed to save task:", error);
      }
    },
  },
  {
    id: "delete",
    icon: "material-delete",
    title: t("TASKS.PAGES.DETAILS.TOOLBAR.DELETE"),
    isVisible: !!props.param && hasAccess(TaskPermissions.Delete),
    clickHandler: async () => {
      if (await showConfirmation(t("TASKS.PAGES.ALERTS.DELETE_CONFIRMATION"))) {
        try {
          await deleteWorkTask();
          emit("parent:call", { method: "reload" });
          emit("close:blade");
        } catch (error) {
          console.error("Failed to delete task:", error);
        }
      }
    },
  },
]);

// Lifecycle hooks
onMounted(async () => {
  await loadWorkTask();
});

onBeforeUnmount(() => {
  unregisterWidget("AssetsWidget", blade?.value.id ?? "");
});

onBeforeClose(async () => {
  if (isModified.value) {
    return await showConfirmation(t("TASKS.PAGES.ALERTS.CLOSE_CONFIRMATION"));
  }
  return true;
});

registerWidget(
  {
    id: "AssetsWidget",
    component: AssetsWidget,
    props: {
      item,
    },
    events: {
      "update:item": (_item: WorkTask) => {
        item.value = _item;
      },
    },
  },
  blade?.value.id ?? "",
);

defineExpose({
  title: bladeTitle,
});
</script>
