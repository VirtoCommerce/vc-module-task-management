import { useUser } from "@vc-shell/framework";
import {
  MemberSearchResult,
  MembersSearchCriteria,
  TaskManagementClient,
} from "../../../../api_client/taskmanagement";
import { computed, Ref, ref } from "vue";
import { CustomerModuleClient, Member } from "../../../../api_client/customer";

interface IUseContacts {
  readonly loading: Ref<boolean>;
  getContact(id: string): Promise<Member>;
  searchContacts(
    keyword?: string,
    skip?: number,
    ids?: string[]
  ): Promise<MemberSearchResult>;
}

export default (): IUseContacts => {
  const loading = ref(false);

  async function getApiClient(): Promise<CustomerModuleClient> {
    const { getAccessToken } = useUser();
    const client = new CustomerModuleClient();
    client.setAuthToken(await getAccessToken());
    return client;
  }

  async function getTaskApiClient(): Promise<TaskManagementClient> {
    const { getAccessToken } = useUser();
    const client = new TaskManagementClient();
    client.setAuthToken(await getAccessToken());
    return client;
  }

  async function getContact(id: string): Promise<Member> {
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

  async function searchContacts(
    keyword?: string,
    skip = 0,
    ids?: string[]
  ): Promise<MemberSearchResult> {
    loading.value = true;
    const client = await getTaskApiClient();
    try {
      const criteria = new MembersSearchCriteria();
      criteria.take = 20;
      criteria.skip = skip;
      criteria.objectIds = ids;
      criteria.memberType = null;
      criteria.memberTypes = ["Contact", "Employee"];
      criteria.deepSearch = true;
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
    getContact,
    searchContacts,
  };
};