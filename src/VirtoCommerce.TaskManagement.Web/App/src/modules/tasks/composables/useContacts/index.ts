import { useUser } from "@vc-shell/framework";
import { computed, Ref, ref } from "vue";
import {
  Contact,
  ContactSearchResult,
  CustomerModuleClient,
  MembersSearchCriteria,
} from "../../../../api_client/customer";

interface IUseContacts {
  readonly loading: Ref<boolean>;
  getContact(id: string): Promise<Contact>;
  searchContacts(
    keyword?: string,
    skip?: number,
    ids?: string[]
  ): Promise<ContactSearchResult>;
}

export default (): IUseContacts => {
  const loading = ref(false);

  async function getApiClient(): Promise<CustomerModuleClient> {
    const { getAccessToken } = useUser();
    const client = new CustomerModuleClient();
    client.setAuthToken(await getAccessToken());
    return client;
  }

  async function getContact(id: string): Promise<Contact> {
    loading.value = true;
    const client = await getApiClient();
    try {
      return await client.getContactById(id);
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
  ): Promise<ContactSearchResult> {
    loading.value = true;
    const client = await getApiClient();
    try {
      const criteria = new MembersSearchCriteria();
      criteria.take = 20;
      criteria.skip = skip;
      criteria.objectIds = ids;
      criteria.memberType = "Contact";
      return await client.searchContacts(criteria);
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
