<template>
  <VcBlade
    v-loading="loading"
    :title="$t('TASKS.PAGES.NEW.TITLE')"
    :expanded="expanded"
    :closable="closable"
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
                name="name"
                class="tw-mb-4"
                :label="$t('TASKS.PAGES.NEW.FIELDS.NAME.TITLE')"
                v-model="newWorkTask.name"
                :clearable="true"
                :placeholder="$t('TASKS.PAGES.NEW.FIELDS.NAME.PLACEHOLDER')"
                maxlength="1024"
                required
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
              rules="required"
              :modelValue="newWorkTask.dueDate"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcInput
                type="date"
                v-bind="field"
                name="dueDate"
                class="tw-mb-4"
                :label="$t('TASKS.PAGES.NEW.FIELDS.DUEDATE.TITLE')"
                v-model="newWorkTask.dueDate"
                :clearable="true"
                :placeholder="$t('TASKS.PAGES.NEW.FIELDS.DUEDATE.PLACEHOLDER')"
                required
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
              name="type"
              rules="required|min:3"
              :modelValue="newWorkTask.type"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcInput
                v-bind="field"
                name="type"
                class="tw-mb-4"
                :label="$t('TASKS.PAGES.NEW.FIELDS.TYPE.TITLE')"
                v-model="newWorkTask.type"
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
          </VcCol>
          <VcCol class="tw-p-1">
            <Field
              name="group"
              rules="required|min:3"
              :modelValue="newWorkTask.group"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcInput
                v-bind="field"
                name="group"
                class="tw-mb-4"
                :label="$t('TASKS.PAGES.NEW.FIELDS.GROUP.TITLE')"
                v-model="newWorkTask.group"
                :clearable="true"
                :placeholder="$t('TASKS.PAGES.NEW.FIELDS.GROUP.PLACEHOLDER')"
                maxlength="1024"
                required
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
              rules="required"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcSelect
                v-bind="field"
                name="priority"
                class="tw-mb-4"
                required
                v-model="newWorkTask.priority"
                option-value="typeName"
                option-label="typeName"
                :label="$t('TASKS.PAGES.NEW.FIELDS.PRIORITY.TITLE')"
                :placeholder="$t('TASKS.PAGES.NEW.FIELDS.PRIORITY.PLACEHOLDER')"
                :clearable="true"
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
              rules="required"
              :modelValue="newWorkTask.responsibleId"
              v-slot="{ field, errorMessage, handleChange, errors }"
            >
              <VcSelect
                v-bind="field"
                name="responsibleId"
                class="tw-mb-4"
                required
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
          </VcCol>
        </VcRow>
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
} from "@vc-shell/framework";
import { WorkTaskPriority } from "../../../api_client/taskmanagement";
import { useIsFormValid, Field } from "vee-validate";
import _ from "lodash";
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
const { workTask, newWorkTask, loading, createWorkTask } = useWorkTask();
const { users, searchUsers } = useUserSearch();
const priorities = Object.values(WorkTaskPriority);

const bladeToolbar = ref<IBladeToolbar[]>([
  {
    title: computed(() => t("TASKS.PAGES.NEW.TOOLBAR.CREATE")),
    icon: "fas fa-check",
    async clickHandler() {
      newWorkTask.value.isActive = true;

      const responsible = _.find(users.value, function (user) {
        if (user.id === newWorkTask.value.responsibleId) {
          return true;
        }
      });

      newWorkTask.value.responsibleName = responsible.userName;

      await createWorkTask(newWorkTask.value);

      emit("parent:call", { method: "reload" });
      emit("close:blade");
      emit("parent:call", {
        method: "onItemClick",
        args: { item: { id: workTask.value.id } },
      });
    },
    disabled: false,
  },
]);

function getCriteria(skip?: number): UserSearchCriteria {
  const criteria = new UserSearchCriteria();

  criteria.take = 20;
  criteria.skip = skip;

  return criteria;
}

onMounted(async () => {
  await searchUsers(getCriteria());
});
</script>
