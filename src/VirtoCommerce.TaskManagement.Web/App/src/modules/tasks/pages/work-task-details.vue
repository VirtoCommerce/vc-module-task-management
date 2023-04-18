<template>
  <VcBlade
    v-loading="loading"
    :title="getTitle()"
    :expanded="expanded"
    :closable="closable"
    width="40%"
    :toolbarItems="bladeToolbar"
    @close="$emit('close:blade')"
  >
    <VcContainer>
      <div class="tw-p-5">
        <VcForm>
          <VcCard class="tw-p-5 tw-mb-2">
            <VcRow>
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.TYPE") }}
                </VcLabel>
                <Field
                  name="type"
                  rules="required|min:3"
                  :modelValue="workTask.type"
                  v-slot="{ field, errorMessage, handleChange, errors }"
                >
                  <VcSelect
                    v-bind="field"
                    class="tw-mb-4"
                    v-model="workTask.type"
                    :clearable="false"
                    :disabled="disabled"
                    option-value="name"
                    option-label="name"
                    :placeholder="$t('TASKS.PAGES.DETAILS.PLACEHOLDER.TYPE')"
                    :error="!!errors.length"
                    :error-message="errorMessage"
                    :options="getTaskTypes"
                    @update:modelValue="handleChange"
                  >
                    <template v-slot:selected-item="item">
                      <span class="tw-ml-1">{{ item.opt.name }}</span>
                    </template>
                    <template v-slot:option="item">
                      <span class="tw-ml-1">{{ item.opt.name }}</span>
                    </template>
                  </VcSelect>
                </Field>
              </VcCol>
            </VcRow>
            <VcRow v-if="!$props.param">
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.NAME") }}
                </VcLabel>
                <Field
                  name="name"
                  rules="required|min:3"
                  :modelValue="workTask.name"
                  v-slot="{ field, errorMessage, handleChange, errors }"
                >
                  <VcInput
                    v-bind="field"
                    class="tw-mb-4"
                    v-model="workTask.name"
                    :clearable="true"
                    required
                    :placeholder="$t('TASKS.PAGES.DETAILS.PLACEHOLDER.NAME')"
                    :error="!!errors.length"
                    :error-message="errorMessage"
                    @update:modelValue="handleChange"
                  >
                  </VcInput>
                </Field>
              </VcCol>
            </VcRow>
            <VcRow class="tw-mb-[15px]">
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.DESCRIPTION") }}
                </VcLabel>
                <Field
                  name="description"
                  :modelValue="workTask.description"
                  v-slot="{ field, errorMessage, handleChange, errors }"
                >
                  <VcEditor
                    v-bind="field"
                    class="tw-mb-4"
                    v-model="workTask.description"
                    :clearable="true"
                    :disabled="disabled"
                    :placeholder="
                      $t('TASKS.PAGES.DETAILS.PLACEHOLDER.DESCRIPTION')
                    "
                    maxlength="10000"
                    :error="!!errors.length"
                    :error-message="errorMessage"
                    @update:modelValue="handleChange"
                    :assets-folder="workTask.id"
                  >
                  </VcEditor>
                </Field>
              </VcCol>
            </VcRow>
            <VcRow>
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.PRIORITY") }}
                </VcLabel>
                <Field
                  name="priority"
                  v-slot="{ field, errorMessage, handleChange, errors }"
                >
                  <VcSelect
                    v-bind="field"
                    class="tw-mb-4"
                    v-model="workTask.priority"
                    option-value="typeName"
                    option-label="typeName"
                    :clearable="false"
                    :disabled="disabled"
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
            </VcRow>
            <VcRow>
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.DUEDATE") }}
                </VcLabel>
                <Field
                  name="dueDate"
                  :modelValue="workTask.dueDate"
                  v-slot="{ field, errorMessage, errors }"
                >
                  <VcInput
                    type="date"
                    v-bind="field"
                    name="dueDate"
                    class="tw-mb-4"
                    :clearable="false"
                    :disabled="disabled"
                    :error="!!errors.length"
                    :error-message="errorMessage"
                    :modelValue="getDueDate()"
                    @update:modelValue="(e : string) => setDueDate(e)"
                  >
                  </VcInput>
                </Field>
              </VcCol>
            </VcRow>
            <VcRow>
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.ASSIGNEE") }}
                </VcLabel>
                <Field
                  name="responsibleId"
                  rules="required"
                  :modelValue="workTask.responsibleId"
                  v-slot="{ field, errorMessage, handleChange, errors }"
                >
                  <VcSelect
                    v-bind="field"
                    name="responsibleId"
                    class="tw-mb-4"
                    required
                    searchable
                    v-model="workTask.responsibleId"
                    option-value="id"
                    option-label="fullName"
                    :clearable="true"
                    :disabled="disabled"
                    :error="!!errors.length"
                    :error-message="errorMessage"
                    :options="searchContacts"
                    @update:modelValue="handleChange"
                  >
                    <template v-slot:selected-item="item">
                      <img
                        class="tw-w-5 tw-h-5 tw-rounded-full"
                        :src="getContactIcon(item.opt.id)"
                        @error="(e) => imgPlaceholder(e)"
                      />
                      <span class="tw-ml-1">{{ item.opt.name }}</span>
                    </template>
                    <template v-slot:option="item">
                      <img
                        class="tw-w-5 tw-h-5 tw-rounded-full"
                        :src="getContactIcon(item.opt.id)"
                        @error="(e) => imgPlaceholder(e)"
                      />
                      <span class="tw-ml-1">{{ item.opt.name }}</span>
                    </template>
                  </VcSelect>
                </Field>
              </VcCol>
            </VcRow>
            <VcRow>
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.STATUS") }}
                </VcLabel>
                <div>
                  <TaskStatus
                    :work-task-status="calculateStatus(workTask)"
                  ></TaskStatus>
                </div>
              </VcCol>
            </VcRow>
          </VcCard>
          <VcCard
            :header="$t('TASKS.PAGES.DETAILS.TASK_INFO.ATTACHMENTS_TITLE')"
            is-collapsable
            :is-collapsed="restoreCollapsed('files')"
            @state:collapsed="handleCollapsed('files', $event)"
            v-if="workTask.isActive === true || workTask.attachments?.length"
          >
            <TaskAttachments
              :workTask="workTask"
              :fileUploading="fileUploading"
              @addAttachments="filesUpload($event)"
              @deleteAttachment="deleteWorkTaskAttachment($event)"
            ></TaskAttachments>
          </VcCard>
        </VcForm>
      </div>
    </VcContainer>
  </VcBlade>
</template>
<script lang="ts">
import moment from "moment";
import {
  useContacts,
  useWorkTask,
  useWorkTaskAttachments,
  useWorkTaskPermissions,
  useWorkTaskTypes,
} from "../composables";
import {
  useI18n,
  IBladeToolbar,
  IParentCallArgs,
  VcBlade,
  VcCol,
  VcContainer,
  VcLabel,
  VcRow,
  VcForm,
  VcInput,
  VcSelect,
  VcEditor,
  VcCard,
} from "@vc-shell/framework";
import { defineComponent, computed, onMounted, ref } from "vue";
import TaskPriority from "../components/taskPriority.vue";
import TaskStatus from "../components/taskStatus.vue";
import { Field, useForm, useIsFormValid } from "vee-validate";
import { forEach } from "lodash";
import TaskAttachments from "../components/taskAttachments.vue";
import { WorkTask } from "../../../api_client/taskmanagement";
import noCustomerIconImage from "/assets/userpic.svg";
import { TaskPermissions } from "../../../types";

export default defineComponent({
  url: "/task",
});
</script>
<script lang="ts" setup>
export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}
export interface Emits {
  (event: "parent:call", args: IParentCallArgs): void;
  (event: "close:blade"): void;
}
const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
  param: null,
});
const emit = defineEmits<Emits>();
const { t } = useI18n();
const {
  workTask,
  loading,
  modified,
  priorities,
  initNewWorkTask,
  loadWorkTask,
  createWorkTask,
  approveWorkTask,
  rejectWorkTask,
  updateWorktask,
  resetWorkTask,
  deleteWorkTask,
} = useWorkTask();
const { getMember, searchContacts } = useContacts();
const { fileUploading, uploadAttachments, deleteAttachment } =
  useWorkTaskAttachments();
const { getTaskTypes } = useWorkTaskTypes();
const { checkWorkTaskPermission } = useWorkTaskPermissions();
useForm({ validateOnMount: false });
const isValid = useIsFormValid();

const disabled = computed(() => !!props.param && !workTask.value.isActive);

onMounted(async () => {
  if (props.param) {
    await loadWorkTask(props.param);
  }
});

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    title: computed(() => t("TASKS.PAGES.DETAILS.ACTIONS.ACCEPT_TASK")),
    icon: "fas fa-check",
    async clickHandler() {
      if (props.param) {
        await approveWorkTask(props.param, workTask.value.parameters);
        emit("parent:call", { method: "reload" });
      }
    },
    isVisible: computed(
      () =>
        !!props.param &&
        workTask.value.isActive === true &&
        checkWorkTaskPermission(TaskPermissions.Complete)
    ),
  },
  {
    title: computed(() => t("TASKS.PAGES.DETAILS.ACTIONS.REJECT_TASK")),
    icon: "fas fa-ban",
    async clickHandler() {
      if (props.param) {
        await rejectWorkTask(props.param, workTask.value.parameters);
        emit("parent:call", { method: "reload" });
      }
    },
    isVisible: computed(
      () =>
        !!props.param &&
        workTask.value.isActive === true &&
        checkWorkTaskPermission(TaskPermissions.Complete)
    ),
  },
  {
    title: computed(() => t("TASKS.PAGES.DETAILS.ACTIONS.RESET_TASK")),
    icon: "fa-regular fa-window-restore",
    async clickHandler() {
      resetWorkTask();
    },
    disabled: computed(() => !modified.value),
    isVisible: computed(
      () =>
        !!props.param &&
        workTask.value.isActive === true &&
        checkWorkTaskPermission(TaskPermissions.Update)
    ),
  },
  {
    title: computed(() => t("TASKS.PAGES.DETAILS.ACTIONS.SAVE_TASK")),
    icon: "fas fa-save",
    async clickHandler() {
      if (props.param) {
        forEach(workTask.value.attachments, function (attachment) {
          if (attachment.id.startsWith("Draft")) {
            attachment.id = null;
          }
        });
        const member = await getMember(workTask.value.responsibleId);
        workTask.value.responsibleName = member?.name;
        await updateWorktask();
        emit("parent:call", { method: "reload" });
      } else {
        const member = await getMember(workTask.value.responsibleId);
        workTask.value.responsibleName = member?.name;
        forEach(workTask.value.attachments, function (attachment) {
          if (attachment.id.startsWith("Draft")) {
            attachment.id = null;
          }
        });
        await createWorkTask();
        emit("parent:call", { method: "reload" });
        emit("close:blade");
      }
    },
    disabled: computed(() => !modified.value || !isValid.value),
    isVisible: computed(
      () =>
        (!!props.param &&
          workTask.value.isActive === true &&
          checkWorkTaskPermission(TaskPermissions.Update)) ||
        (!props.param &&
          workTask.value.isActive === true &&
          checkWorkTaskPermission(TaskPermissions.Create))
    ),
  },
  {
    title: computed(() => t("TASKS.PAGES.DETAILS.ACTIONS.DELETE_TASK")),
    icon: "fas fa-trash",
    async clickHandler() {
      if (props.param) {
        await deleteWorkTask(props.param);
        emit("parent:call", { method: "reload" });
        emit("close:blade");
      }
    },
    isVisible: computed(
      () => !!props.param && checkWorkTaskPermission(TaskPermissions.Delete)
    ),
  },
]);

const getTitle = () => {
  return props.param
    ? "# " + workTask.value.number + ": " + workTask.value.name
    : t("TASKS.PAGES.DETAILS.NEW_TITLE");
};

const filesUpload = async (files: FileList) => {
  await uploadAttachments(files, workTask);
};

const deleteWorkTaskAttachment = (id: string) => {
  deleteAttachment(id, workTask);
};

function handleCollapsed(key: string, value: boolean): void {
  localStorage?.setItem(key, `${value}`);
}
function restoreCollapsed(key: string): boolean {
  return localStorage?.getItem(key) === "true";
}
function setDueDate(value: string) {
  const date = moment(value).toDate();
  if (date instanceof Date && !isNaN(date.valueOf())) {
    workTask.value.dueDate = date;
  } else {
    workTask.value.dueDate = undefined;
  }
}
function getDueDate() {
  const date = workTask.value.dueDate;
  if (date) {
    return moment(date).format("YYYY-MM-DD");
  }
  return undefined;
}

const getContactIcon = (id: string) => {
  return "/api/task-management/contact/icon/" + id;
};

const calculateStatus = (workTask: WorkTask) => {
  let result = "ToDo";
  if (workTask.isActive === true) {
    switch (workTask.completed) {
      case false:
      case true:
        result = "Canceled";
        break;
    }
  } else {
    switch (workTask.completed) {
      case null:
      case false:
        result = "Canceled";
        break;
      case true:
        result = "Done";
        break;
    }
  }
  return result;
};

function imgPlaceholder(e: Event) {
  e.target["src"] = noCustomerIconImage;
}

async function onBeforeClose() {
  initNewWorkTask();
}

defineExpose({
  onBeforeClose,
});
</script>

<style lang="scss">
.attachment-wrap {
  overflow-wrap: anywhere;
}
.delete-icon {
  position: absolute;
  top: 2px;
  right: 0px;
  width: 20px;
  height: 20px;
  line-height: 20px;
  text-align: center;
  border: 0;
  font-weight: bold;
  font-size: 20px;
  cursor: pointer;
}
</style>
