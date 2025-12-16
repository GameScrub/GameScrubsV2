<template>
  <div class="flex h-screen overflow-hidden">
    <!-- Sidebar -->
    <Sidebar :sidebarOpen="sidebarOpen" @close-sidebar="sidebarOpen = false" variant="v2" />

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
          <!-- Page content -->
          <div class="mx-auto flex flex-col lg:flex-row lg:space-x-8 xl:space-x-16">
            <!-- Content -->
            <div>
              <header class="mb-4">
                <!-- Title -->
                <h1 class="text-2xl md:text-3xl text-gray-800 dark:text-gray-100 font-bold">
                  About GameScrubs.com
                </h1>
              </header>

              <!-- Tags -->
              <div class="mb-6">
                <div class="flex flex-wrap items-center -m-1">
                  <div class="m-1">
                    <a
                      class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 hover:border-gray-300 dark:hover:border-gray-600 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                      href="#0"
                    >
                      .Net Core
                    </a>
                  </div>
                  <div class="m-1">
                    <a
                      class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 hover:border-gray-300 dark:hover:border-gray-600 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                      href="#0"
                    >
                      MSSQL
                    </a>
                  </div>
                  <div class="m-1">
                    <a
                      class="btn-xs text-xs border-gray-200 dark:border-gray-700/60 hover:border-gray-300 dark:hover:border-gray-600 text-gray-800 dark:text-gray-300 px-2.5 py-1 rounded-full shadow-none"
                      href="#0"
                    >
                      Vue
                    </a>
                  </div>
                </div>
              </div>

              <hr class="my-6 border-t border-gray-100 dark:border-gray-700/60" />

              <!-- The Role -->
              <div>
                <h2 class="text-xl leading-snug text-gray-800 dark:text-gray-100 font-bold mb-2">
                  Lorem
                </h2>
                <div class="space-y-6">
                  <p>
                    Bacon ipsum dolor amet cow hamburger filet mignon, pork chop t-bone picanha pork
                    belly biltong brisket swine andouille tri-tip. Porchetta tri-tip swine biltong
                    drumstick buffalo cow meatloaf. Ham hock ground round pork doner pig jowl
                    biltong turducken burgdoggen bresaola bacon pastrami. Spare ribs kevin sausage
                    ribeye burgdoggen venison ground round short ribs jerky frankfurter. Beef ribs
                    bresaola buffalo frankfurter swine leberkas rump jerky.
                  </p>
                  <p>
                    Strip steak biltong sausage fatback shank leberkas shoulder beef venison spare
                    ribs. Beef ribs drumstick alcatra kielbasa landjaeger turducken sausage boudin
                    hamburger beef shoulder turkey. Filet mignon tongue hamburger meatloaf, ribeye
                    ball tip picanha corned beef ham hock spare ribs kielbasa swine rump. Doner cow
                    sirloin frankfurter cupim picanha filet mignon turducken ham hock pancetta
                    fatback prosciutto buffalo pork belly tenderloin.
                  </p>
                </div>
              </div>

              <hr class="my-6 border-t border-gray-100 dark:border-gray-700/60" />

              <!-- About You -->
              <div>
                <h2 class="text-xl leading-snug text-gray-800 dark:text-gray-100 font-bold mb-2">
                  Bacon
                </h2>
                <div class="space-y-6">
                  <p>
                    Biltong pig drumstick burgdoggen kielbasa. Burgdoggen pork ground round
                    landjaeger ribeye, leberkas sausage pork loin tail ball tip drumstick. Picanha
                    shank pancetta ribeye bresaola. Pork loin meatball andouille jerky shoulder
                    burgdoggen prosciutto short ribs flank salami boudin. Shankle turducken sirloin
                    chuck kielbasa. Picanha spare ribs sirloin pork pig, shank tail burgdoggen
                    corned beef. Pig flank salami meatloaf, pork loin picanha burgdoggen alcatra
                    pastrami kevin.
                  </p>
                </div>
              </div>

              <hr class="my-6 border-t border-gray-100 dark:border-gray-700/60" />

              <!-- Things You Might Do -->
              <div>
                <h2 class="text-xl leading-snug text-gray-800 dark:text-gray-100 font-bold mb-2">
                  Ipsum
                </h2>
                <div class="space-y-6">
                  <p>
                    Sausage pork hamburger capicola chuck bacon, ball tip jowl meatball frankfurter
                    buffalo pork belly. Pancetta burgdoggen beef shankle salami short loin tri-tip
                    sausage jowl turducken corned beef ribeye pork chop t-bone. Rump picanha
                    meatball, ground round frankfurter strip steak prosciutto beef ribs jowl shankle
                    ham sausage boudin. T-bone strip steak shankle venison, beef ground round
                    frankfurter. Strip steak pancetta picanha, drumstick pork chop hamburger spare
                    ribs jerky capicola bresaola biltong pork belly ham meatball cow. Salami
                    prosciutto tri-tip andouille biltong, alcatra capicola.
                  </p>
                  <ul class="list-disc list-inside space-y-1">
                    <li>Doner kevin ribeye beef ribs</li>
                    <li>short loin prosciutto ground round corned beef</li>
                  </ul>
                </div>
              </div>
            </div>
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

const sidebarOpen: Ref<boolean> = ref(false);
</script>
