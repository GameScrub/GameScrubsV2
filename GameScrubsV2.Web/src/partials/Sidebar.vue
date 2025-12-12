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
          <IconArrowLeft :size="24" :stroke-width="1.5" />
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
            <SidebarLinkGroup v-slot="parentLink" :activeCondition="true">
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
                    <IconTournament
                      :size="26"
                      :stroke-width="1.5"
                      :class="
                        currentRoute.fullPath === '/' || currentRoute.fullPath.includes('dashboard')
                          ? 'text-violet-500'
                          : 'text-gray-400 dark:text-gray-500'
                      "
                    />
                    <span
                      class="text-sm font-medium ml-4 lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                    >
                      Tournaments
                    </span>
                  </div>
                  <!-- Icon -->
                  <div class="flex shrink-0 ml-2">
                    <IconChevronDown
                      :size="12"
                      :stroke-width="1.5"
                      :class="[
                        'text-gray-400 dark:text-gray-500 transition-transform',
                        parentLink.expanded && 'rotate-180',
                      ]"
                    />
                  </div>
                </div>
              </a>
              <div class="lg:hidden lg:sidebar-expanded:block 2xl:block">
                <ul class="pl-8 mt-1" :class="!parentLink.expanded && 'hidden'">
                  <router-link to="/bracket-list" custom v-slot="{ href, navigate, isExactActive }">
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
                          Bracket List
                        </span>
                      </a>
                    </li>
                  </router-link>
                  <router-link
                    to="/bracket/create"
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
                          Create a Bracket
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
            <SidebarLinkGroup>
              <div>
                <div class="flex items-center">
                  <IconBrandLinkedin
                    :size="26"
                    :stroke-width="1.5"
                    class="text-gray-400 dark:text-gray-500"
                  />
                  <span
                    class="text-sm font-medium ml-4 lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                  >
                    <a href="https://www.linkedin.com/in/rartigas/" target="_blank">
                      Rafael Artigas
                    </a>
                  </span>
                </div>
                <div class="flex items-center pt-2">
                  <router-link to="/about" class="flex items-center">
                    <IconFileStar
                      :size="26"
                      :stroke-width="1.5"
                      class="text-gray-400 dark:text-gray-500"
                    />
                    <span
                      class="text-sm font-medium ml-4 lg:opacity-0 lg:sidebar-expanded:opacity-100 2xl:opacity-100 duration-200"
                    >
                      About
                    </span>
                  </router-link>
                </div>
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
            <IconChevronRight
              :size="16"
              :stroke-width="1.5"
              :class="[
                'text-gray-400 dark:text-gray-500 transition-transform',
                sidebarExpanded && 'rotate-180',
              ]"
            />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, type Ref } from 'vue';
import { useRouter, type RouteLocationNormalizedLoaded } from 'vue-router';
import {
  IconArrowLeft,
  IconTournament,
  IconFileStar,
  IconChevronDown,
  IconChevronRight,
  IconBrandLinkedin,
} from '@tabler/icons-vue';
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
