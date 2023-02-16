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
            <VcRow class="tw-mb-[15px]">
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.DESCRIPTION") }}
                </VcLabel>
                <Field
                  v-if="workTask.isActive == true"
                  name="description"
                  rules="required|min:3"
                  :modelValue="workTask.description"
                  v-slot="{ field, errorMessage, handleChange, errors }"
                >
                  <VcTextarea
                    v-bind="field"
                    class="tw-mb-4"
                    v-model="workTask.description"
                    :clearable="true"
                    :placeholder="
                      $t('TASKS.PAGES.NEW.FIELDS.DESCRIPTION.PLACEHOLDER')
                    "
                    maxlength="1024"
                    required
                    :error="!!errors.length"
                    :error-message="errorMessage"
                    @update:modelValue="handleChange"
                  >
                  </VcTextarea>
                </Field>
                <div class="tw-text-base" v-else>
                  {{ workTask.description }}
                </div>
              </VcCol>
            </VcRow>
            <VcRow>
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.PRIORITY") }}
                </VcLabel>
              </VcCol>
              <VcCol>
                <Field
                  v-if="workTask.isActive === true"
                  name="priority"
                  rules="required"
                  v-slot="{ field, errorMessage, handleChange, errors }"
                >
                  <VcSelect
                    v-bind="field"
                    name="priority"
                    class="tw-mb-4"
                    required
                    v-model="workTask.priority"
                    option-value="typeName"
                    option-label="typeName"
                    :clearable="false"
                    :error="!!errors.length"
                    :error-message="errorMessage"
                    :options="priorities"
                    @update:modelValue="handleChange"
                  >
                    <template v-slot:selected-item="item">
                      <TaskPriority
                        :workTaskPriority="item.opt"
                        :withText="true"
                      ></TaskPriority>
                    </template>
                    <template v-slot:option="item">
                      <TaskPriority
                        :workTaskPriority="item.opt"
                        :withText="true"
                      ></TaskPriority>
                    </template>
                  </VcSelect>
                </Field>
                <div class="tw-text-base" v-else>
                  <TaskPriority
                    :workTaskPriority="workTask.priority"
                    :withText="true"
                  ></TaskPriority>
                </div>
              </VcCol>
            </VcRow>
            <VcRow>
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.DUEDATE") }}
                </VcLabel>
              </VcCol>
              <VcCol>
                <Field
                  v-if="workTask.isActive"
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
                    :placeholder="
                      $t('TASKS.PAGES.NEW.FIELDS.DUEDATE.PLACEHOLDER')
                    "
                    :error="!!errors.length"
                    :error-message="errorMessage"
                    :modelValue="getDueDate()"
                    @update:modelValue="setDueDate($event)"
                  >
                  </VcInput>
                </Field>
                <div class="tw-text-base" v-else>
                  {{ moment(workTask.dueDate).format("MMM DD, YYYY") }}
                </div>
              </VcCol>
            </VcRow>
            <VcRow>
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.TYPE") }}
                </VcLabel>
              </VcCol>
              <VcCol>
                <Field
                  v-if="workTask.isActive"
                  name="type"
                  rules="required|min:3"
                  :modelValue="workTask.type"
                  v-slot="{ field, errorMessage, handleChange, errors }"
                >
                  <VcInput
                    v-bind="field"
                    name="type"
                    class="tw-mb-4"
                    v-model="workTask.type"
                    :clearable="true"
                    :placeholder="$t('TASKS.PAGES.NEW.FIELDS.TYPE.PLACEHOLDER')"
                    maxlength="1024"
                    required
                    :error="!!errors.length"
                    :error-message="errorMessage"
                    @update:modelValue="handleChange"
                  >
                  </VcInput>
                </Field>
                <div class="tw-text-base" v-else>
                  {{ workTask.type }}
                </div>
              </VcCol>
            </VcRow>
            <VcRow>
              <VcCol>
                <VcLabel class="tw-mb-2 tw-text-lg">
                  {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.STATUS") }}
                </VcLabel>
              </VcCol>
              <VcCol class="tw-p-1">
                <div>
                  <TaskStatus
                    :active="workTask.isActive"
                    :completed="workTask.completed"
                  ></TaskStatus>
                </div>
              </VcCol>
            </VcRow>
          </VcCard>
          <VcCard
            :header="$t('TASKS.PAGES.DETAILS.TASK_INFO.CREDENTIALS_TITLE')"
            is-collapsable
            :is-collapsed="restoreCollapsed('credentials')"
            @state:collapsed="handleCollapsed('credentials', $event)"
          >
            <VcRow class="tw-p-5">
              <VcCol>
                <div class="tw-p-3">
                  <VcLabel class="tw-mb-2">
                    {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.ASSIGNEE") }}
                  </VcLabel>
                </div>
              </VcCol>
              <VcCol class="tw-p-1">
                <Field
                  v-if="workTask.isActive"
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
                    v-model="workTask.responsibleId"
                    option-value="id"
                    option-label="userName"
                    :clearable="false"
                    :error="!!errors.length"
                    :error-message="errorMessage"
                    :options="users"
                    @update:modelValue="handleChange"
                  >
                  </VcSelect>
                </Field>
                <div class="tw-p-3" v-else>
                  {{ workTask.responsibleName }}
                </div>
              </VcCol>
            </VcRow>
            <VcRow class="tw-p-5">
              <VcCol class="tw-p-1">
                <div class="tw-p-3">
                  <VcLabel class="tw-mb-2">
                    {{ $t("TASKS.PAGES.DETAILS.TASK_INFO.REPORTER") }}
                  </VcLabel>
                </div>
              </VcCol>
              <VcCol class="tw-p-1">
                <div class="tw-p-3">
                  {{ workTask.createdBy }}
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
            <div class="tw-flex tw-flex-wrap tw-p-5">
              <div class="tw-flex tw-flex-wrap tw-flex-1">
                <div
                  class="vc-file-upload vc-file-upload_gallery tw-relative tw-h-[155px] tw-box-border tw-border tw-border-dashed tw-border-[#c8dbea] tw-rounded-[6px] tw-p-4 tw-m-2 tw-flex tw-flex-col tw-items-center tw-justify-center"
                  v-for="attachment in workTask.attachments"
                  v-bind:key="attachment.id"
                >
                  <i
                    class="vc-icon vc-icon_s fa-solid fa-xmark tw-text-[#c8dbea] hover:tw-text-[color:var(--app-bar-button-color-hover)] delete-icon"
                    @click="deleteAttachment(attachment)"
                    v-if="workTask.isActive === true"
                  ></i>
                  <i
                    class="vc-icon vc-icon_xxl fa-solid fa-file tw-text-[#c8dbea]"
                  ></i>
                  <div
                    class="tw-text-[#9db0be] tw-text-center tw-text-lg tw-leading-lg tw-mt-4"
                  >
                    <a
                      class="vc-link attachment-wrap"
                      :href="attachment.url"
                      target="_blank"
                      >{{ attachment.name }}</a
                    >
                  </div>
                </div>
              </div>
            </div>
            <div class="tw-flex tw-flex-wrap tw-p-5">
              <VcFileUpload
                @upload="fileUpload"
                v-if="workTask.isActive === true"
                class="tw-m-2"
              ></VcFileUpload>
              <VcLoading :active="fileUploading"></VcLoading>
            </div>
          </VcCard>
        </VcForm>
      </div>
    </VcContainer>
  </VcBlade>
</template>
<script lang="ts">
import moment from "moment";
import { useUserSearch, useWorkTask } from "../composables";
import {
  useI18n,
  IBladeToolbar,
  IParentCallArgs,
  useUser,
  VcBlade,
  VcCol,
  VcContainer,
  VcLabel,
  VcRow,
  VcForm,
  VcInput,
  VcSelect,
  UserSearchCriteria,
  VcTextarea,
  VcFileUpload,
  VcCard,
} from "@vc-shell/framework";
import { defineComponent, computed, onMounted, ref } from "vue";
import TaskPriority from "../components/taskPriority.vue";
import TaskStatus from "../components/taskStatus.vue";
import { Field } from "vee-validate";
import { WorkTaskAttachment } from "../../../api_client/taskmanagement";
import filter from "lodash-es/filter";
import uniqueId from "lodash-es/uniqueId";
import { forEach } from "lodash";
export default defineComponent({
  url: "task",
  components: { VcFileUpload, VcCard },
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
  param: undefined,
});
const emit = defineEmits<Emits>();
const { t } = useI18n();
const {
  workTask,
  loading,
  modified,
  priorities,
  loadWorkTask,
  approveWorkTask,
  rejectWorkTask,
  updateWorktask,
  resetWorkTask,
  deleteWorkTask,
} = useWorkTask();
const { users, searchUsers } = useUserSearch();
const { user, getAccessToken } = useUser();
const fileUploading = ref(false);

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
    isVisible: computed(() => workTask.value.isActive === true),
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
    isVisible: computed(() => workTask.value.isActive === true),
  },
  {
    title: computed(() => t("TASKS.PAGES.DETAILS.ACTIONS.RESET_TASK")),
    icon: "fa-regular fa-window-restore",
    async clickHandler() {
      resetWorkTask();
    },
    disabled: computed(() => !modified.value),
    isVisible: computed(() => workTask.value.isActive === true),
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
        await updateWorktask();
        emit("parent:call", { method: "reload" });
      }
    },
    disabled: computed(() => !modified.value),
    isVisible: computed(() => workTask.value.isActive === true),
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
    isVisible: computed(() => workTask.value.createdBy === user.value.userName),
  },
]);
const getTitle = () => {
  return "# " + workTask.value.number + ": " + workTask.value.name;
};
function getCriteria(skip?: number): UserSearchCriteria {
  const criteria = new UserSearchCriteria();
  criteria.take = 20;
  criteria.skip = skip;
  return criteria;
}

const fileUpload = async (files: FileList) => {
  try {
    fileUploading.value = true;
    for (let i = 0; i < files.length; i++) {
      const formData = new FormData();
      formData.append("file", files[i]);
      const authToken = await getAccessToken();
      const result = await fetch(
        `/api/assets?folderUrl=/workTaskAttachment/${workTask.value.id}`,
        {
          method: "POST",
          body: formData,
          headers: {
            Authorization: `Bearer ${authToken}`,
          },
        }
      );
      const response = await result.json();
      if (response?.length) {
        const attachment = new WorkTaskAttachment(response[0]);
        attachment.id = uniqueId("Draft-");
        attachment.createdDate = new Date();
        workTask.value.attachments.push(attachment);
      }
    }
  } catch (e) {
    console.log(e);
  } finally {
    fileUploading.value = false;
  }
  files = null;
};
const deleteAttachment = (attachment: WorkTaskAttachment) => {
  workTask.value.attachments = filter(workTask.value.attachments, function (a) {
    if (attachment.id) {
      return a.id !== attachment.id;
    }
  });
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
onMounted(async () => {
  await searchUsers(getCriteria());
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
