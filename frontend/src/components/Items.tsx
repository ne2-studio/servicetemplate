import { useState } from 'react';
import { useItemStore } from '../store/useItemStore';
import { Plus, Trash2 } from 'lucide-react';

export function Items() {
  const { items, addItem, deleteItem } = useItemStore();
  const [isAdding, setIsAdding] = useState(false);
  const [newItem, setNewItem] = useState({ name: '', description: '' });

  const handleAddItem = () => {
    if (!newItem.name) return;
    addItem(newItem);
    setIsAdding(false);
    setNewItem({ name: '', description: '' });
  };

  const formatDate = (val: string) =>
    new Intl.DateTimeFormat('en-US', { dateStyle: 'medium' }).format(new Date(val));

  return (
    <div className="flex flex-col gap-8">
      <div className="flex items-center justify-between">
        <div className="flex flex-col gap-1">
          <h2 className="text-lg font-medium tracking-tight text-text-primary">Items</h2>
          <p className="text-xs text-text-secondary">Example screen demonstrating the store → api → types layering.</p>
        </div>
        <button
          onClick={() => setIsAdding(true)}
          className="bg-primary text-white px-4 py-1.5 text-xs font-bold uppercase tracking-widest rounded-sm hover:bg-primary/90 transition-all flex items-center gap-2"
        >
          <Plus className="w-3.5 h-3.5" /> New item
        </button>
      </div>

      {isAdding && (
        <div className="bg-surface border border-border p-6 rounded-sm">
          <h3 className="text-[10px] font-mono uppercase tracking-widest text-text-secondary mb-6">Create item</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
            <div className="flex flex-col gap-2">
              <label className="text-[10px] font-mono uppercase tracking-wider text-text-secondary">Name</label>
              <input
                type="text"
                value={newItem.name}
                onChange={e => setNewItem(prev => ({ ...prev, name: e.target.value }))}
                className="bg-background border border-border p-2 text-sm rounded-sm focus:ring-1 focus:ring-primary outline-none"
              />
            </div>
            <div className="flex flex-col gap-2">
              <label className="text-[10px] font-mono uppercase tracking-wider text-text-secondary">Description</label>
              <input
                type="text"
                value={newItem.description}
                onChange={e => setNewItem(prev => ({ ...prev, description: e.target.value }))}
                className="bg-background border border-border p-2 text-sm rounded-sm focus:ring-1 focus:ring-primary outline-none"
              />
            </div>
          </div>
          <div className="flex justify-end gap-3">
            <button onClick={() => setIsAdding(false)} className="px-6 py-2 text-xs font-bold uppercase tracking-widest text-text-secondary hover:text-text-primary transition-all">Cancel</button>
            <button onClick={handleAddItem} className="bg-primary text-white px-6 py-2 text-xs font-bold uppercase tracking-widest rounded-sm hover:bg-primary/90 transition-all">Create item</button>
          </div>
        </div>
      )}

      <div className="bg-surface border border-border rounded-sm overflow-hidden">
        <table className="w-full text-left">
          <thead>
            <tr className="bg-surface-elevated/50 border-b border-border">
              <th className="p-4">Name</th>
              <th className="p-4">Description</th>
              <th className="p-4">Created</th>
              <th className="p-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-border/30">
            {items.map((item) => (
              <tr key={item.id} className="hover:bg-surface-elevated/20 transition-colors">
                <td className="p-4 text-sm font-medium">{item.name}</td>
                <td className="p-4 text-sm text-text-secondary">{item.description}</td>
                <td className="p-4 text-xs font-mono text-text-secondary">{formatDate(item.createdAt)}</td>
                <td className="p-4 text-right">
                  <button onClick={() => deleteItem(item.id)} className="p-1.5 text-text-secondary hover:text-error transition-all">
                    <Trash2 className="w-3.5 h-3.5" />
                  </button>
                </td>
              </tr>
            ))}
            {items.length === 0 && (
              <tr>
                <td colSpan={4} className="p-12 text-center text-text-secondary italic text-xs">
                  No items yet.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
