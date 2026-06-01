<template>
  <VcBlade
    :loading="loading"
    :title="bladeTitle"
    width="50%"
    :toolbar-items="toolbarItems"
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
        <VcSelect
          v-model="item.priority"
          :label="$t('TASKS.PAGES.DETAILS.FIELDS.PRIORITY.TITLE')"
          :options="priorities"
          option-value="value"
          option-label="name"
          :disabled="isReadOnly"
        >
          <template #selected-item="{ opt }">
            <SelectPriority :item="(opt as IPriority).value" />
          </template>
          <template #option="{ opt }">
            <SelectPriority :item="(opt as IPriority).value" />
          </template>
        </VcSelect>

        <!-- Due Date -->
        <VcInput
          v-model="item.dueDate"
          type="datetime-local"
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
import { computed, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { Field } from "vee-validate";
import { IBladeToolbar, useBlade, useBladeForm, usePermissions, usePopup } from "@vc-shell/framework";

import { useWorkTaskDetails, type IPriority } from "../composables/useWorkTask";
import { useTasksWidgets } from "../widgets/useTasksWidgets";
import { TaskPermissions } from "../../../types";
import SelectPriority from "../components/selectPriority.vue";

import { VcBlade, VcContainer, VcEditor, VcForm, VcInput, VcSelect, VcStatus } from "@vc-shell/framework/ui";

defineBlade({
  name: "WorkTaskDetails",
  url: "/work-task-details",
});

const { hasAccess } = usePermissions();
const { param, callParent, closeSelf } = useBlade();
const { showConfirmation } = usePopup();
const { t } = useI18n({ useScope: "global" });

const {
  item,
  loading,
  saveWorkTask,
  completeWorkTask,
  rejectWorkTask,
  deleteWorkTask,
  isReadOnly,
  priorities,
  loadTaskTypes,
  searchContacts,
  statusText,
  loadWorkTask,
} = useWorkTaskDetails({
  id: param.value,
  isNew: !param.value,
});

const form = useBladeForm({
  data: item,
  closeConfirmMessage: computed(() => t("TASKS.PAGES.ALERTS.CLOSE_CONFIRMATION")),
});

// Register widgets
useTasksWidgets({
  item,
  isVisible: computed(() => !!param.value),
});

// Blade title
const bladeTitle = computed(() => {
  return param.value ? `# ${item.value?.number}: ${item.value?.name}` : t("TASKS.PAGES.DETAILS.NEW_TITLE");
});

// Reactive toolbar
const toolbarItems = computed((): IBladeToolbar[] => [
  {
    id: "complete",
    icon: "lucide-check",
    title: t("TASKS.PAGES.DETAILS.TOOLBAR.COMPLETE"),
    isVisible: !!param.value && item.value?.isActive && hasAccess(TaskPermissions.Finish),
    clickHandler: async () => {
      try {
        await completeWorkTask();
        form.setBaseline();
        callParent("reload");
      } catch (error) {
        console.error("Failed to complete task:", error);
      }
    },
  },
  {
    id: "reject",
    icon: "lucide-ban",
    title: t("TASKS.PAGES.DETAILS.TOOLBAR.REJECT"),
    isVisible: !!param.value && item.value?.isActive && hasAccess(TaskPermissions.Finish),
    clickHandler: async () => {
      try {
        await rejectWorkTask();
        form.setBaseline();
        callParent("reload");
      } catch (error) {
        console.error("Failed to reject task:", error);
      }
    },
  },
  {
    id: "reset",
    icon: "lucide-undo",
    title: t("TASKS.PAGES.DETAILS.TOOLBAR.RESET"),
    disabled: !form.isModified.value,
    isVisible: !!param.value && item.value?.isActive && hasAccess(TaskPermissions.Update),
    clickHandler: () => {
      form.revert();
    },
  },
  {
    id: "save",
    icon: "lucide-save",
    title: t("TASKS.PAGES.DETAILS.TOOLBAR.SAVE"),
    disabled: !form.canSave.value,
    isVisible:
      (param.value && item.value?.isActive && hasAccess(TaskPermissions.Update)) ||
      (!param.value && item.value?.isActive && hasAccess(TaskPermissions.Create)),
    clickHandler: async () => {
      try {
        await saveWorkTask();
        form.setBaseline();
        callParent("reload");
        callParent("onItemClick", item.value);
      } catch (error) {
        console.error("Failed to save task:", error);
      }
    },
  },
  {
    id: "delete",
    icon: "lucide-trash-2",
    title: t("TASKS.PAGES.DETAILS.TOOLBAR.DELETE"),
    isVisible: !!param.value && hasAccess(TaskPermissions.Delete),
    clickHandler: async () => {
      if (await showConfirmation(t("TASKS.PAGES.ALERTS.DELETE_CONFIRMATION"))) {
        try {
          await deleteWorkTask();
          callParent("reload");
          closeSelf();
        } catch (error) {
          console.error("Failed to delete task:", error);
        }
      }
    },
  },
]);

onMounted(async () => {
  await loadWorkTask();
  form.setBaseline();
});
</script>
