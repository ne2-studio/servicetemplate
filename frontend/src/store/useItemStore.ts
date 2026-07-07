import { create } from 'zustand';
import { Item } from '../types';
import { api } from '../api';

interface ItemStore {
  items: Item[];
  isLoading: boolean;
  error: string | null;

  fetchData: () => Promise<void>;

  addItem: (item: Omit<Item, 'id' | 'createdAt'>) => Promise<Item>;
  updateItem: (id: string, updates: Partial<Item>) => Promise<void>;
  deleteItem: (id: string) => Promise<void>;
}

export const useItemStore = create<ItemStore>((set) => ({
  items: [],
  isLoading: false,
  error: null,

  fetchData: async () => {
    set({ isLoading: true, error: null });
    try {
      const [items] = await Promise.all([
        api.items.getAll(),
      ]);

      set({ items, isLoading: false });
    } catch (error) {
      set({ error: (error as Error).message, isLoading: false });
    }
  },

  addItem: async (item) => {
    const newItem = await api.items.create(item);
    set((state) => ({ items: [newItem, ...state.items] }));
    return newItem;
  },

  updateItem: async (id, updates) => {
    await api.items.update(id, updates);
    set((state) => ({
      items: state.items.map((i) => (i.id === id ? new Item({ ...i, ...updates }) : i)),
    }));
  },

  deleteItem: async (id) => {
    await api.items.delete(id);
    set((state) => ({ items: state.items.filter((i) => i.id !== id) }));
  },
}));
