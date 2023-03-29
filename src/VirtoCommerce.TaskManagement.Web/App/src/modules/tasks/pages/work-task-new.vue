<template>
  <VcBlade
    v-loading="loading"
    :title="$t('TASKS.PAGES.NEW.TITLE')"
    :expanded="props.expanded"
    :closable="props.closable"
    width="30%"
    :toolbarItems="bladeToolbar"
    @close="$emit('close:blade')"
  >
    <VcContainer>
      <VcForm>
        <VcRow class="tw-p-1">
          <VcCol class="tw-p-1">
            <Field
              name="name"
              rules="required|min:3"
              :modelValue="newWorkTask.name"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcInput
                v-bind="field"
                class="tw-mb-4"
                :label="$t('TASKS.PAGES.NEW.FIELDS.NAME.TITLE')"
                v-model="newWorkTask.name"
                :clearable="true"
                required
                :placeholder="$t('TASKS.PAGES.NEW.FIELDS.NAME.PLACEHOLDER')"
                :error="!!errors.length"
                :error-message="errorMessage"
                @update:modelValue="handleChange"
              >
              </VcInput>
            </Field>
          </VcCol>
        </VcRow>
        <VcRow class="tw-p-1">
          <VcCol class="tw-p-1">
            <Field
              name="description"
              rules="required|min:3"
              :modelValue="newWorkTask.description"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcTextarea
                v-bind="field"
                class="tw-mb-4"
                :label="$t('TASKS.PAGES.NEW.FIELDS.DESCRIPTION.TITLE')"
                v-model="newWorkTask.description"
                :clearable="true"
                required
                :placeholder="
                  $t('TASKS.PAGES.NEW.FIELDS.DESCRIPTION.PLACEHOLDER')
                "
                :error="!!errors.length"
                :error-message="errorMessage"
                @update:modelValue="handleChange"
              >
              </VcTextarea>
            </Field>
          </VcCol>
        </VcRow>
        <VcRow class="tw-p-1">
          <VcCol class="tw-p-1">
            <Field
              name="type"
              rules="required|min:3"
              :modelValue="newWorkTask.type"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcInput
                v-bind="field"
                class="tw-mb-4"
                :label="$t('TASKS.PAGES.NEW.FIELDS.TYPE.TITLE')"
                v-model="newWorkTask.type"
                :clearable="true"
                required
                :placeholder="$t('TASKS.PAGES.NEW.FIELDS.TYPE.PLACEHOLDER')"
                maxlength="1024"
                :error="!!errors.length"
                :error-message="errorMessage"
                @update:modelValue="handleChange"
              >
              </VcInput>
            </Field>
          </VcCol>
          <VcCol class="tw-p-1">
            <Field
              name="dueDate"
              :modelValue="newWorkTask.dueDate"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcInput
                type="date"
                v-bind="field"
                class="tw-mb-4"
                :label="$t('TASKS.PAGES.NEW.FIELDS.DUEDATE.TITLE')"
                v-model="newWorkTask.dueDate"
                :clearable="true"
                :placeholder="$t('TASKS.PAGES.NEW.FIELDS.DUEDATE.PLACEHOLDER')"
                :error="!!errors.length"
                :error-message="errorMessage"
                @update:modelValue="handleChange"
              >
              </VcInput>
            </Field>
          </VcCol>
        </VcRow>
        <VcRow class="tw-p-1">
          <VcCol class="tw-p-1">
            <Field
              name="priority"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcSelect
                v-bind="field"
                class="tw-mb-4"
                v-model="newWorkTask.priority"
                option-value="typeName"
                option-label="typeName"
                :label="$t('TASKS.PAGES.NEW.FIELDS.PRIORITY.TITLE')"
                :placeholder="$t('TASKS.PAGES.NEW.FIELDS.PRIORITY.PLACEHOLDER')"
                :error="!!errors.length"
                :error-message="errorMessage"
                :options="priorities"
                @update:modelValue="handleChange"
              >
                <template v-slot:selected-item="item">
                  <TaskPriority :workTaskPriority="item.opt"></TaskPriority>
                </template>
                <template v-slot:option="item">
                  <TaskPriority :workTaskPriority="item.opt"></TaskPriority>
                </template>
              </VcSelect>
            </Field>
          </VcCol>
          <VcCol class="tw-p-1">
            <Field
              name="responsibleId"
              :modelValue="newWorkTask.responsibleId"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcSelect
                v-bind="field"
                class="tw-mb-4"
                v-model="newWorkTask.responsibleId"
                option-value="id"
                option-label="fullName"
                :label="$t('TASKS.PAGES.NEW.FIELDS.ASSIGNEDTO.TITLE')"
                :placeholder="
                  $t('TASKS.PAGES.NEW.FIELDS.ASSIGNEDTO.PLACEHOLDER')
                "
                :clearable="true"
                :error="!!errors.length"
                :error-message="errorMessage"
                :options="searchContacts"
                @update:modelValue="handleChange"
              >
                <template v-slot:selected-item="item">
                  <img
                    class="tw-w-5 tw-h-5 tw-rounded-full"
                    :src="getContactIcon(item.opt.id)"
                    onerror="javascript:this.src='/assets/userpic.svg'"
                  />
                  <span class="tw-ml-1">{{ item.opt.name }}</span>
                </template>
                <template v-slot:option="item">
                  <img
                    class="tw-w-5 tw-h-5 tw-rounded-full"
                    :src="getContactIcon(item.opt.id)"
                    onerror="javascript:this.src='/assets/userpic.svg'"
                  />
                  <span class="tw-ml-1">{{ item.opt.name }}</span>
                </template>
              </VcSelect>
            </Field>
          </VcCol>
        </VcRow>
        <VcCard
          :header="$t('TASKS.PAGES.DETAILS.TASK_INFO.ATTACHMENTS_TITLE')"
          is-collapsable
          :is-collapsed="restoreCollapsed('files')"
          @state:collapsed="handleCollapsed('files', $event)"
        >
          <TaskAttachments
            :workTask="newWorkTask"
            :fileUploading="fileUploading"
            @addAttachments="filesUpload($event)"
            @deleteAttachment="deleteWorkTaskAttachment($event)"
          ></TaskAttachments>
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>
<script lang="ts">
import { computed, ref } from "vue";
import {
  VcInput,
  useI18n,
  IBladeToolbar,
  VcTextarea,
  IParentCallArgs,
  VcBlade,
  VcContainer,
  VcForm,
  VcRow,
  VcCol,
  VcSelect,
  VcCard,
} from "@vc-shell/framework";
import { WorkTask, WorkTaskPriority } from "../../../api_client/taskmanagement";
import { Field, useForm, useIsFormValid } from "vee-validate";
import { forEach } from "lodash";
import TaskPriority from "../components/taskPriority.vue";
import TaskAttachments from "../components/taskAttachments.vue";
</script>

<script lang="ts" setup>
import {
  useContacts,
  useWorkTask,
  useWorkTaskAttachments,
} from "../composables";
export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}
export interface Emits {
  (event: "close:blade"): void;
  (event: "parent:call", args: IParentCallArgs): void;
}
const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
  param: undefined,
});
const emit = defineEmits<Emits>();
const { t } = useI18n();
const { workTask, loading, createWorkTask } = useWorkTask();
const { getMember, searchContacts } = useContacts();
const { fileUploading, uploadAttachments, deleteAttachment } =
  useWorkTaskAttachments();
const priorities = Object.values(WorkTaskPriority);
const newWorkTask = ref({
  priority: WorkTaskPriority.Normal,
  attachments: [],
  isActive: true,
} as WorkTask);
useForm({ validateOnMount: false });
const isValid = useIsFormValid();

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    title: computed(() => t("TASKS.PAGES.NEW.TOOLBAR.CREATE")),
    icon: "fas fa-save",
    async clickHandler() {
      newWorkTask.value.isActive = true;
      const member = await getMember(newWorkTask.value.responsibleId);
      newWorkTask.value.responsibleName = member?.name;
      forEach(newWorkTask.value.attachments, function (attachment) {
        if (attachment.id.startsWith("Draft")) {
          attachment.id = null;
        }
      });
      await createWorkTask(newWorkTask.value);
      newWorkTask.value = {} as WorkTask;
      emit("parent:call", { method: "reload" });
      emit("close:blade");
      emit("parent:call", {
        method: "onItemClick",
        args: { id: workTask.value.id },
      });
    },
    disabled: computed(() => isValid.value === false),
  },
]);

const filesUpload = async (files: FileList) => {
  await uploadAttachments(files, newWorkTask);
};

const deleteWorkTaskAttachment = (id: string) => {
  deleteAttachment(id, newWorkTask);
};

function handleCollapsed(key: string, value: boolean): void {
  localStorage?.setItem(key, `${value}`);
}
function restoreCollapsed(key: string): boolean {
  return localStorage?.getItem(key) === "true";
}

const getContactIcon = (id: string) => {
  return "/api/task-management/contact/icon/" + id;
};
</script>
