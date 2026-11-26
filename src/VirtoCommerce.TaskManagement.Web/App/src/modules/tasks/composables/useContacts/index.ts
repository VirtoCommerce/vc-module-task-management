import { useApiClient } from "@vc-shell/framework";
import {
  TaskMember,
  TaskMemberSearchResult,
  MembersSearchCriteria,
  TaskManagementClient,
} from "../../../../api_client/virtocommerce.taskmanagement";
import { computed, Ref, ref } from "vue";

interface IUseContacts {
  readonly loading: Ref<boolean>;
  getMember(id: string): Promise<TaskMember>;
  searchContacts(keyword?: string, skip?: number, ids?: string[]): Promise<TaskMemberSearchResult>;
}

const { getApiClient } = useApiClient(TaskManagementClient);

export default (): IUseContacts => {
  const loading = ref(false);

  async function getMember(id: string): Promise<TaskMember> {
    loading.value = true;
    const client = await getApiClient();
    try {
      return await client.getMemberById(id);
    } catch (e) {
      console.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  async function searchContacts(keyword?: string, skip = 0, ids?: string[]): Promise<TaskMemberSearchResult> {
    loading.value = true;
    const client = await getApiClient();
    try {
      const criteria = {
        keyword,
        take: 20,
        skip,
        objectIds: ids ?? [],
        memberType: undefined,
        memberTypes: ["Contact", "Employee"],
        deepSearch: true,
      } as MembersSearchCriteria;
      return await client.searchAssignMembers(criteria);
    } catch (e) {
      console.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  return {
    loading: computed(() => loading.value),
    getMember,
    searchContacts,
  };
};
