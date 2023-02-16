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
                option-label="userName"
                :label="$t('TASKS.PAGES.NEW.FIELDS.ASSIGNEDTO.TITLE')"
                :placeholder="
                  $t('TASKS.PAGES.NEW.FIELDS.ASSIGNEDTO.PLACEHOLDER')
                "
                :clearable="true"
                :error="!!errors.length"
                :error-message="errorMessage"
                :options="users"
                @update:modelValue="handleChange"
              >
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
          <div class="tw-flex tw-flex-wrap tw-p-5">
            <div class="tw-flex tw-flex-wrap tw-flex-1">
              <div
                class="vc-file-upload vc-file-upload_gallery tw-relative tw-h-[155px] tw-box-border tw-border tw-border-dashed tw-border-[#c8dbea] tw-rounded-[6px] tw-p-4 tw-m-2 tw-flex tw-flex-col tw-items-center tw-justify-center"
                v-for="attachment in newWorkTask.attachments"
                v-bind:key="attachment.id"
              >
                <i
                  class="vc-icon vc-icon_s fa-solid fa-xmark tw-text-[#c8dbea] hover:tw-text-[color:var(--app-bar-button-color-hover)] delete-icon"
                  @click="deleteAttachment(attachment)"
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
            <VcFileUpload @upload="fileUpload" class="tw-m-2"></VcFileUpload>
            <VcLoading :active="fileUploading"></VcLoading>
          </div>
        </VcCard>
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>
<script lang="ts">
import { computed, onMounted, ref } from "vue";
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
  UserSearchCriteria,
  VcSelect,
  VcCard,
  VcFileUpload,
  VcLoading,
  useUser,
} from "@vc-shell/framework";
import {
  WorkTask,
  WorkTaskAttachment,
  WorkTaskPriority,
} from "../../../api_client/taskmanagement";
import { Field, useForm, useIsFormValid } from "vee-validate";
import _, { filter, forEach, remove, uniqueId } from "lodash";
</script>

<script lang="ts" setup>
import { useWorkTask, useUserSearch } from "../composables";
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
const { users, searchUsers } = useUserSearch();
const priorities = Object.values(WorkTaskPriority);
const newWorkTask = ref({
  priority: WorkTaskPriority.Normal,
  attachments: [],
} as WorkTask);
useForm({ validateOnMount: false });
const isValid = useIsFormValid();
const { getAccessToken } = useUser();
const fileUploading = ref(false);
const defaultTaskAttachmentFolder = "Draft";
const bladeToolbar = ref<IBladeToolbar[]>([
  {
    title: computed(() => t("TASKS.PAGES.NEW.TOOLBAR.CREATE")),
    icon: "fas fa-save",
    async clickHandler() {
      newWorkTask.value.isActive = true;
      const responsible = _.find(users.value, function (user) {
        if (user.id === newWorkTask.value.responsibleId) {
          return true;
        }
      });
      newWorkTask.value.responsibleName = responsible?.userName;
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
        `/api/assets?folderUrl=/workTaskAttachment/${defaultTaskAttachmentFolder}`,
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
        newWorkTask.value.attachments.push(attachment);
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
  console.log(newWorkTask.value.attachments);
  console.log(attachment.id);
  newWorkTask.value.attachments = filter(
    newWorkTask.value.attachments,
    function (a) {
      if (attachment.id) {
        return a.id !== attachment.id;
      }
    }
  );
  console.log(newWorkTask.value.attachments);
};
function handleCollapsed(key: string, value: boolean): void {
  localStorage?.setItem(key, `${value}`);
}
function restoreCollapsed(key: string): boolean {
  return localStorage?.getItem(key) === "true";
}
onMounted(async () => {
  await searchUsers(getCriteria());
});
</script>
