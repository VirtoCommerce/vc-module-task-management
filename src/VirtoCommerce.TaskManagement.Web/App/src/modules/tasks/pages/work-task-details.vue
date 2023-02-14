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
                  v-slot="{ field, errorMessage, handleChange, errors }"
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

          <VcRow class="tw-p-1">
            <VcCol class="tw-p-1">
              <div class="tw-p-3">
                <TaskStatus
                  :active="workTask.isActive"
                  :completed="workTask.completed"
                ></TaskStatus>
              </div>
            </VcCol>
          </VcRow>
          <VcCard
            :header="$t('TASKS.PAGES.DETAILS.TASK_INFO.ATTACHMENTS_TITLE')"
            is-collapsable
            :is-collapsed="restoreCollapsed('files')"
            @state:collapsed="handleCollapsed('files', $event)"
            v-if="workTask.isActive === true || workTask.attachments?.length"
          >
            <TaskAttachments :workTask="workTask"></TaskAttachments>
          </VcCard>
        </VcForm>
      </div>
    </VcContainer>
  </VcBlade>
</template>

<script lang="ts">
import moment from "moment";

import {
  useUserSearch,
  useWorkTask,
  useWorkTaskAttachments,
} from "../composables";
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
  VcCard,
} from "@vc-shell/framework";
import { defineComponent, computed, onMounted, ref } from "vue";
import TaskPriority from "../components/taskPriority.vue";
import TaskStatus from "../components/taskStatus.vue";
import TaskAttachments from "../components/taskAttachments.vue";
import { Field } from "vee-validate";
import { forEach } from "lodash";

export default defineComponent({
  url: "task",
  components: { VcCard },
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
const { user } = useUser();

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
