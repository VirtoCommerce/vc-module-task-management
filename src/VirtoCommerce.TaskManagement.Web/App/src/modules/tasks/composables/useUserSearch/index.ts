import { computed, Ref, ref } from "vue";
import {
  ApplicationUser,
  SecurityClient,
  useLogger,
  UserSearchCriteria,
  UserSearchResult,
  useUser,
} from "@vc-shell/framework";

interface IUseUserSearch {
  readonly users: Ref<ApplicationUser[]>;
  readonly totalCount: Ref<number>;
  readonly pages: Ref<number>;
  readonly loading: Ref<boolean>;
  readonly currentPage: Ref<number>;
  searchUsers(criteria?: UserSearchCriteria): void;
}

export default (): IUseUserSearch => {
  const logger = useLogger();
  const loading = ref(false);
  const usersSearchResult = ref(new UserSearchResult({ results: [] }));
  const currentPage = ref(1);

  async function getApiClient(): Promise<SecurityClient> {
    const { getAccessToken } = useUser();
    const client = new SecurityClient();
    client.setAuthToken(await getAccessToken());
    return client;
  }

  async function searchUsers(criteria?: UserSearchCriteria) {
    loading.value = true;
    const client = await getApiClient();
    try {
      usersSearchResult.value = await client.searchUsers(criteria);
      currentPage.value =
        (criteria?.skip || 0) / Math.max(1, criteria?.take || 20) + 1;
    } catch (e) {
      logger.error(e);
      throw e;
    } finally {
      loading.value = false;
    }
  }

  return {
    users: computed(() => usersSearchResult.value?.results),
    totalCount: computed(() => usersSearchResult.value?.totalCount),
    pages: computed(() => Math.ceil(usersSearchResult.value?.totalCount / 20)),
    loading: computed(() => loading.value),
    currentPage: computed(() => currentPage.value),
    searchUsers,
  };
};
