import { createRouter, createWebHistory } from 'vue-router';
import BracketListView from '@/views/BracketListView.vue';
import HomeView from '@/views/HomeView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      name: 'home',
      path: '/',
      component: HomeView,
    },
    {
      name: 'bracket-list',
      path: '/bracket-list',
      component: BracketListView,
    },
    {
      name: 'bracket-create',
      path: '/bracket/create',
      component: () => import('../views/ManageBracketView.vue'),
    },
    {
      name: 'bracket-edit',
      path: '/bracket/:id/edit',
      component: () => import('../views/ManageBracketView.vue'),
    },
    {
      name: 'bracket-manage-users',
      path: '/bracket/:id/manage-users',
      component: () => import('../views/ManageUserBracketView.vue'),
    },
    {
      name: 'bracket',
      path: '/bracket/:id',
      component: () => import('../views/BracketView.vue'),
    },
    {
      name: 'about',
      path: '/about',
      component: () => import('../views/AboutView.vue'),
    },
  ],
});

export default router;
