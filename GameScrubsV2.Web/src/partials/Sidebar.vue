<template>
  <div class="min-w-fit">
    <!-- Sidebar backdrop (mobile only) -->
    <div
      class="fixed inset-0 bg-gray-900/30 z-40 lg:hidden lg:z-auto transition-opacity duration-200"
      :class="sidebarOpen ? 'opacity-100' : 'opacity-0 pointer-events-none'"
      aria-hidden="true"
    ></div>

    <!-- Sidebar -->
    <div
      id="sidebar"
      ref="sidebar"
      class="flex lg:flex! flex-col absolute z-40 left-0 top-0 lg:static lg:left-auto lg:top-auto lg:translate-x-0 h-[100dvh] overflow-y-scroll lg:overflow-y-auto no-scrollbar w-64 lg:w-20 lg:sidebar-expanded:!w-64 2xl:w-64! shrink-0 bg-white dark:bg-gray-800 p-4 transition-all duration-200 ease-in-out"
      :class="[
        variant === 'v2'
          ? 'border-r border-gray-200 dark:border-gray-700/60'
          : 'rounded-r-2xl shadow-xs',
        sidebarOpen ? 'translate-x-0' : '-translate-x-64',
      ]"
    >
      <!-- Sidebar header -->
      <div class="flex justify-between mb-10 pr-3 sm:px-2">
        <!-- Close button -->
        <button
          ref="trigger"
          class="lg:hidden text-gray-500 hover:text-gray-400"
          @click.stop="$emit('close-sidebar')"
          aria-controls="sidebar"
          :aria-expanded="sidebarOpen"
        >
          <span class="sr-only">Close sidebar</span>
          <svg class="w-6 h-6 fill-current" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path d="M10.7 18.7l1.4-1.4L7.8 13H20v-2H7.8l4.3-4.3-1.4-1.4L4 12z" />
          </svg>
        </button>
        <!-- Logo -->
        <router-link class="block" to="/">
          <img src="../images/GameScrubsLogo.png" height="32" />
        </router-link>
      </div>

      <!-- Links -->
      <div class="space-y-8">
        <!-- Pages group -->
        <div>
          <h3 class="text-xs uppercase text-gray-400 dark:text-gray-500 font-semibold pl-3">
            <span
              class="hidden lg:block lg:sidebar-expanded:hidden 2xl:hidden text-center w-6"
              aria-hidden="true"
            >
              •••
            </span>
            <span class="lg:hidden lg:sidebar-expanded:block 2xl:block">Pages</span>
          </h3>
          <ul class="mt-3">
            <!-- Dashboard -->
            <SidebarLinkGroup
              v-slot="parentLink"
              :activeCondition="
                currentRoute.fullPath === '/' || currentRoute.fullPath.includes('dashboard')
              "
            >
              <a
                class="block text-gray-800 dark:text-gray-100 truncate transition"
                :class="
                  currentRoute.fullPath === '/' || currentRoute.fullPath.includes('dashboard')
                    ? ''
                    : 'hover:text-gray-900 dark:hover:text-white'
                "
                href="#0"
                @click.prevent="
                  parentLink.handleClick();
                  sidebarExpanded = true;
                "
              >
                <div class="flex items-center justify-between">
                  <div class="flex items-center">
                    <svg
                      class="shrink-0 fill-current"
                      :class="
                        currentRoute.fullPath === '/' || currentRoute.fullPath.includes('dashboard')
                          ? 'text-violet-500'
                          : 'text-gray-400 dark:text-gray-500'
                      "
                      xmlns="http://www.w3.org/2000/svg"
                      width="16"
                      height="16"
                      viewBox="0 0 16 16"
                    >
                      <path
                        d="M5.936.278A7.983 7.983 0 0 1 8 0a8 8 0 1 1-8 8c0-.722.104-1.413.278-2.064a1 1 0 1 1 1.932.516A5.99 5.99 0 0 0 2 8a6 6 0 1 0 6-6c-.53 0-1.045.076-1.548.21A1 1 0 1 1 5.936.278Z"
                      />
                      <path
                        d="M6.068 7.482A2.003 2.003 0 0 0 8 10a2 2 0 1 0-.518-3.932L3.707 2.293a1 1 0 0 0-1.414 1.414l3.775 3.775Z"
                      />
                    </svg>
                    <span
                      class="text-sm font-medium ml-4 lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                    >
                      Dashboard
                    </span>
                  </div>
                  <!-- Icon -->
                  <div class="flex shrink-0 ml-2">
                    <svg
                      class="w-3 h-3 shrink-0 ml-1 fill-current text-gray-400 dark:text-gray-500"
                      :class="parentLink.expanded && 'rotate-180'"
                      viewBox="0 0 12 12"
                    >
                      <path d="M5.9 11.4L.5 6l1.4-1.4 4 4 4-4L11.3 6z" />
                    </svg>
                  </div>
                </div>
              </a>
              <div class="lg:hidden lg:sidebar-expanded:block 2xl:block">
                <ul class="pl-8 mt-1" :class="!parentLink.expanded && 'hidden'">
                  <router-link to="/" custom v-slot="{ href, navigate, isExactActive }">
                    <li class="mb-1 last:mb-0">
                      <a
                        class="block transition truncate"
                        :class="
                          isExactActive
                            ? 'text-violet-500'
                            : 'text-gray-500/90 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200'
                        "
                        :href="href"
                        @click="navigate"
                      >
                        <span
                          class="text-sm font-medium lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                        >
                          Main
                        </span>
                      </a>
                    </li>
                  </router-link>
                  <router-link
                    to="/dashboard/analytics"
                    custom
                    v-slot="{ href, navigate, isExactActive }"
                  >
                    <li class="mb-1 last:mb-0">
                      <a
                        class="block transition truncate"
                        :class="
                          isExactActive
                            ? 'text-violet-500'
                            : 'text-gray-500/90 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200'
                        "
                        :href="href"
                        @click="navigate"
                      >
                        <span
                          class="text-sm font-medium lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                        >
                          Analytics
                        </span>
                      </a>
                    </li>
                  </router-link>
                  <router-link
                    to="/dashboard/fintech"
                    custom
                    v-slot="{ href, navigate, isExactActive }"
                  >
                    <li class="mb-1 last:mb-0">
                      <a
                        class="block transition truncate"
                        :class="
                          isExactActive
                            ? 'text-violet-500'
                            : 'text-gray-500/90 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200'
                        "
                        :href="href"
                        @click="navigate"
                      >
                        <span
                          class="text-sm font-medium lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                        >
                          Fintech
                        </span>
                      </a>
                    </li>
                  </router-link>
                </ul>
              </div>
            </SidebarLinkGroup>
          </ul>
        </div>
        <!-- More group -->
        <div>
          <h3 class="text-xs uppercase text-gray-400 dark:text-gray-500 font-semibold pl-3">
            <span
              class="hidden lg:block lg:sidebar-expanded:hidden 2xl:hidden text-center w-6"
              aria-hidden="true"
            >
              •••
            </span>
            <span class="lg:hidden lg:sidebar-expanded:block 2xl:block">More</span>
          </h3>
          <ul class="mt-3">
            <!-- Authentication -->
            <SidebarLinkGroup v-slot="parentLink">
              <a
                class="block text-gray-800 dark:text-gray-100 truncate transition"
                :class="parentLink.expanded ? '' : 'hover:text-gray-900 dark:hover:text-white'"
                href="#0"
                @click.prevent="
                  parentLink.handleClick();
                  sidebarExpanded = true;
                "
              >
                <div class="flex items-center justify-between">
                  <div class="flex items-center">
                    <svg
                      class="shrink-0 fill-current text-gray-400 dark:text-gray-500"
                      xmlns="http://www.w3.org/2000/svg"
                      width="16"
                      height="16"
                      viewBox="0 0 16 16"
                    >
                      <path
                        d="M11.442 4.576a1 1 0 1 0-1.634-1.152L4.22 11.35 1.773 8.366A1 1 0 1 0 .227 9.634l3.281 4a1 1 0 0 0 1.59-.058l6.344-9ZM15.817 4.576a1 1 0 1 0-1.634-1.152l-5.609 7.957a1 1 0 0 0-1.347 1.453l.656.8a1 1 0 0 0 1.59-.058l6.344-9Z"
                      />
                    </svg>
                    <span
                      class="text-sm font-medium ml-4 lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                    >
                      Authentication
                    </span>
                  </div>
                  <!-- Icon -->
                  <div class="flex shrink-0 ml-2">
                    <svg
                      class="w-3 h-3 shrink-0 ml-1 fill-current text-gray-400 dark:text-gray-500"
                      :class="parentLink.expanded && 'rotate-180'"
                      viewBox="0 0 12 12"
                    >
                      <path d="M5.9 11.4L.5 6l1.4-1.4 4 4 4-4L11.3 6z" />
                    </svg>
                  </div>
                </div>
              </a>
              <div class="lg:hidden lg:sidebar-expanded:block 2xl:block">
                <ul class="pl-8 mt-1" :class="!parentLink.expanded && 'hidden'">
                  <router-link to="/signin" custom v-slot="{ href, navigate }">
                    <li class="mb-1 last:mb-0">
                      <a
                        class="block text-gray-500/90 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200 transition truncate"
                        :href="href"
                        @click="navigate"
                      >
                        <span
                          class="text-sm font-medium lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                        >
                          Sign in
                        </span>
                      </a>
                    </li>
                  </router-link>
                  <router-link to="/signup" custom v-slot="{ href, navigate }">
                    <li class="mb-1 last:mb-0">
                      <a
                        class="block text-gray-500/90 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200 transition truncate"
                        :href="href"
                        @click="navigate"
                      >
                        <span
                          class="text-sm font-medium lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                        >
                          Sign up
                        </span>
                      </a>
                    </li>
                  </router-link>
                  <router-link to="/reset-password" custom v-slot="{ href, navigate }">
                    <li class="mb-1 last:mb-0">
                      <a
                        class="block text-gray-500/90 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200 transition truncate"
                        :href="href"
                        @click="navigate"
                      >
                        <span
                          class="text-sm font-medium lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                        >
                          Reset Password
                        </span>
                      </a>
                    </li>
                  </router-link>
                </ul>
              </div>
            </SidebarLinkGroup>
          </ul>
        </div>
      </div>

      <!-- Expand / collapse button -->
      <div class="pt-3 hidden lg:inline-flex 2xl:hidden justify-end mt-auto">
        <div class="w-12 pl-4 pr-3 py-2">
          <button
            class="text-gray-400 hover:text-gray-500 dark:text-gray-500 dark:hover:text-gray-400"
            @click.prevent="sidebarExpanded = !sidebarExpanded"
          >
            <span class="sr-only">Expand / collapse sidebar</span>
            <svg
              class="shrink-0 fill-current text-gray-400 dark:text-gray-500 sidebar-expanded:rotate-180"
              xmlns="http://www.w3.org/2000/svg"
              width="16"
              height="16"
              viewBox="0 0 16 16"
            >
              <path
                d="M15 16a1 1 0 0 1-1-1V1a1 1 0 1 1 2 0v14a1 1 0 0 1-1 1ZM8.586 7H1a1 1 0 1 0 0 2h7.586l-2.793 2.793a1 1 0 1 0 1.414 1.414l4.5-4.5A.997.997 0 0 0 12 8.01M11.924 7.617a.997.997 0 0 0-.217-.324l-4.5-4.5a1 1 0 0 0-1.414 1.414L8.586 7M12 7.99a.996.996 0 0 0-.076-.373Z"
              />
            </svg>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, type Ref } from 'vue';
import { useRouter, type RouteLocationNormalizedLoaded } from 'vue-router';
import SidebarLinkGroup from './SidebarLinkGroup.vue';

interface Props {
  sidebarOpen: boolean;
  variant?: string;
}

interface Emits {
  (e: 'close-sidebar'): void;
}

const props = defineProps<Props>();
const emit = defineEmits<Emits>();

const trigger: Ref<HTMLElement | null> = ref(null);
const sidebar: Ref<HTMLElement | null> = ref(null);

const storedSidebarExpanded: string | null = localStorage.getItem('sidebar-expanded');
const sidebarExpanded: Ref<boolean> = ref(
  storedSidebarExpanded === null ? false : storedSidebarExpanded === 'true',
);

const router = useRouter();
const currentRoute: RouteLocationNormalizedLoaded = router.currentRoute.value;

// close on click outside
const clickHandler = ({ target }: MouseEvent): void => {
  if (!sidebar.value || !trigger.value) return;
  if (
    !props.sidebarOpen ||
    sidebar.value.contains(target as Node) ||
    trigger.value.contains(target as Node)
  )
    return;
  emit('close-sidebar');
};

// close if the esc key is pressed
const keyHandler = ({ keyCode }: KeyboardEvent): void => {
  if (!props.sidebarOpen || keyCode !== 27) return;
  emit('close-sidebar');
};

onMounted(() => {
  document.addEventListener('click', clickHandler);
  document.addEventListener('keydown', keyHandler);
});

onUnmounted(() => {
  document.removeEventListener('click', clickHandler);
  document.removeEventListener('keydown', keyHandler);
});

watch(sidebarExpanded, () => {
  localStorage.setItem('sidebar-expanded', sidebarExpanded.value.toString());
  if (sidebarExpanded.value) {
    document.querySelector('body')?.classList.add('sidebar-expanded');
  } else {
    document.querySelector('body')?.classList.remove('sidebar-expanded');
  }
});
</script>
