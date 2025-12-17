<template>
  <div class="flex h-screen overflow-hidden">
    <!-- Sidebar -->
    <Sidebar
      :sidebarOpen="sidebarOpen"
      @close-sidebar="sidebarOpen = false"
      :variant="SidebarVariant.V2"
    />

    <!-- Content area -->
    <div class="relative flex flex-col flex-1 overflow-y-auto overflow-x-hidden">
      <!-- Header -->
      <Header
        :sidebarOpen="sidebarOpen"
        :variant="HeaderVariant.Bracket"
        @toggle-sidebar="sidebarOpen = !sidebarOpen"
      />

      <!-- Main content -->
      <main class="grow">
        <div class="px-4 sm:px-6 lg:px-8 py-8 w-full max-w-9xl mx-auto">
          <!-- Page header -->
          <header class="mb-6">
            <h1 class="text-2xl md:text-3xl text-gray-800 dark:text-gray-100 font-bold">
              About GameScrubs.com
            </h1>
          </header>

          <!-- Page content -->
          <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
            <!-- Main content - Left Column -->
            <div class="lg:col-span-8">
              <!-- What We Do -->
              <div>
                <h2 class="text-xl leading-snug text-gray-800 dark:text-gray-100 font-bold mb-3">
                  What We Do
                </h2>
                <div class="space-y-4">
                  <p class="text-gray-600 dark:text-gray-400">
                    GameScrubs is a web-based platform designed to simplify tournament bracket
                    management for organizers of all experience levels. Whether you're running a
                    local gaming tournament, a sports competition, or an esports event, we provide
                    the tools you need to create, manage, and track your brackets with ease.
                  </p>
                  <p class="text-gray-600 dark:text-gray-400">
                    Our platform supports both single and double elimination formats for 8, 16, or
                    32 players, with automatic seeding and bye handling. Real-time bracket updates
                    ensure all participants and spectators stay informed as matches progress.
                  </p>
                </div>
              </div>

              <hr class="my-6 border-t border-gray-100 dark:border-gray-700/60" />

              <!-- Our Mission -->
              <div>
                <h2 class="text-xl leading-snug text-gray-800 dark:text-gray-100 font-bold mb-3">
                  Our Mission
                </h2>
                <div class="space-y-4">
                  <p class="text-gray-600 dark:text-gray-400">
                    We believe organizing tournaments shouldn't require complex software or
                    technical expertise. Our mission is to provide a simple, secure, and
                    professional platform that anyone can use to run successful tournaments.
                  </p>
                  <p class="text-gray-600 dark:text-gray-400">
                    From casual gaming nights to competitive esports events, GameScrubs handles the
                    bracket management so you can focus on what matters most, the competition and
                    the community.
                  </p>
                </div>
              </div>

              <hr class="my-6 border-t border-gray-100 dark:border-gray-700/60" />

              <!-- About the Developer -->
              <div>
                <h2 class="text-xl leading-snug text-gray-800 dark:text-gray-100 font-bold mb-3">
                  About the Developer
                </h2>
                <div class="space-y-5">
                  <p class="text-gray-600 dark:text-gray-400">
                    GameScrubs was built by
                    <span class="font-semibold text-gray-800 dark:text-gray-100">
                      Rafael Artigas
                    </span>
                    , a Senior Software Engineer with
                    <span class="font-semibold text-gray-800 dark:text-gray-100">
                      12+ years of professional .NET experience
                    </span>
                    creating robust, scalable applications across diverse industries, from gaming
                    giants like Tempo and Blizzard Entertainment to innovative companies like
                    Carvana and Tivity Health.
                  </p>

                  <!-- Key Achievements -->
                  <div class="bg-gray-50 dark:bg-gray-900/50 rounded-lg p-5 space-y-3">
                    <h3 class="text-base font-semibold text-gray-800 dark:text-gray-100 mb-3">
                      Professional Highlights
                    </h3>
                    <ul class="space-y-2.5">
                      <li class="flex items-start gap-2 text-sm text-gray-600 dark:text-gray-400">
                        <svg
                          class="w-5 h-5 text-blue-600 dark:text-blue-400 shrink-0 mt-0.5"
                          fill="none"
                          stroke="currentColor"
                          viewBox="0 0 24 24"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                          />
                        </svg>
                        <span>
                          Architected payment processing systems at Tempo handling
                          <span class="font-medium text-gray-800 dark:text-gray-100">
                            millions of dollars in transactions
                          </span>
                          for 'The Bazaar' game
                        </span>
                      </li>
                      <li class="flex items-start gap-2 text-sm text-gray-600 dark:text-gray-400">
                        <svg
                          class="w-5 h-5 text-green-600 dark:text-green-400 shrink-0 mt-0.5"
                          fill="none"
                          stroke="currentColor"
                          viewBox="0 0 24 24"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                          />
                        </svg>
                        <span>
                          Modernized Blizzard's Employee Management Platform, improving page load
                          performance from
                          <span class="font-medium text-gray-800 dark:text-gray-100">
                            8 seconds to under 1 second
                          </span>
                        </span>
                      </li>
                      <li class="flex items-start gap-2 text-sm text-gray-600 dark:text-gray-400">
                        <svg
                          class="w-5 h-5 text-purple-600 dark:text-purple-400 shrink-0 mt-0.5"
                          fill="none"
                          stroke="currentColor"
                          viewBox="0 0 24 24"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                          />
                        </svg>
                        <span>
                          Wrote and maintained
                          <span class="font-medium text-gray-800 dark:text-gray-100">
                            2,000+ integration and unit tests
                          </span>
                          using NUnit, NSubstitute, and Testcontainers
                        </span>
                      </li>
                      <li class="flex items-start gap-2 text-sm text-gray-600 dark:text-gray-400">
                        <svg
                          class="w-5 h-5 text-orange-600 dark:text-orange-400 shrink-0 mt-0.5"
                          fill="none"
                          stroke="currentColor"
                          viewBox="0 0 24 24"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                          />
                        </svg>
                        <span>
                          Reduced errors by
                          <span class="font-medium text-gray-800 dark:text-gray-100">50%</span>
                          at Carvana through caching strategies, circuit breaking, and comprehensive
                          logging
                        </span>
                      </li>
                      <li class="flex items-start gap-2 text-sm text-gray-600 dark:text-gray-400">
                        <svg
                          class="w-5 h-5 text-indigo-600 dark:text-indigo-400 shrink-0 mt-0.5"
                          fill="none"
                          stroke="currentColor"
                          viewBox="0 0 24 24"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                          />
                        </svg>
                        <span>
                          Led development teams and mentored junior developers across multiple
                          organizations
                        </span>
                      </li>
                    </ul>
                  </div>

                  <p class="text-gray-600 dark:text-gray-400">
                    With deep expertise spanning
                    <span class="font-medium text-gray-800 dark:text-gray-100">
                      C#, ASP.NET Core, Entity Framework Core and Azure cloud services
                    </span>
                    , Rafael applies SOLID principles and comprehensive testing practices to every
                    project. His experience includes building RESTful APIs serving hundreds of
                    thousands of concurrent users, implementing microservices architectures, and
                    managing production deployments using Azure CLI and EF Core migrations.
                  </p>

                  <p class="text-gray-600 dark:text-gray-400">
                    This project represents the perfect intersection of professional expertise and
                    personal passion, combining over a decade of enterprise software development
                    with a genuine love for competitive gaming. GameScrubs embodies the same
                    principles Rafael applies in Fortune 500 environments:
                    <span class="font-medium text-gray-800 dark:text-gray-100">
                      performance optimization, security-first design, and user-centric solutions
                      that simply work
                    </span>
                    .
                  </p>
                </div>
              </div>
            </div>

            <!-- Sidebar - Right Column -->
            <aside class="lg:col-span-4">
              <div
                class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6 border border-gray-200 dark:border-gray-700/60"
              >
                <h3 class="text-lg font-semibold text-gray-800 dark:text-gray-100 mb-4">
                  Built with Modern Technologies
                </h3>
                <div class="space-y-4">
                  <!-- Backend Technologies -->
                  <div>
                    <p class="text-xs font-medium text-gray-500 dark:text-gray-400 mb-2">Backend</p>
                    <div class="flex flex-wrap items-center -m-1">
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          .NET 10
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          ASP.NET Core Web API
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          Entity Framework Core 10
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          ASP.NET Identity
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          JWT Authentication
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          Serilog
                        </span>
                      </div>
                    </div>
                  </div>

                  <!-- Database -->
                  <div>
                    <p class="text-xs font-medium text-gray-500 dark:text-gray-400 mb-2">
                      Database
                    </p>
                    <div class="flex flex-wrap items-center -m-1">
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          Microsoft SQL Server
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          EF Core Migrations
                        </span>
                      </div>
                    </div>
                  </div>

                  <!-- Frontend Technologies -->
                  <div>
                    <p class="text-xs font-medium text-gray-500 dark:text-gray-400 mb-2">
                      Frontend
                    </p>
                    <div class="flex flex-wrap items-center -m-1">
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          Vue 3.5
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          TypeScript 5.9
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          Composition API
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          Vue Router 4
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          Pinia 3
                        </span>
                      </div>
                      <div class="m-1">
                        <span
                          class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                        >
                          Vite 7
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </aside>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, type Ref } from 'vue';
import Sidebar from '@/partials/Sidebar.vue';
import Header from '@/partials/Header.vue';
import { HeaderVariant } from '@/models/HeaderVariant';
import { SidebarVariant } from '@/models/SidebarVariant';

const sidebarOpen: Ref<boolean> = ref(false);
</script>
