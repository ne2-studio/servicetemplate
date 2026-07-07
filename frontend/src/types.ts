// Example domain entity demonstrating the class-per-entity convention.
// Replace with the real domain model(s) for this project.
export class Item {
  id: string;
  name: string;
  description?: string;
  createdAt: string;

  constructor(data: {
    id: string;
    name: string;
    description?: string;
    createdAt: string;
  }) {
    this.id = data.id;
    this.name = data.name;
    this.description = data.description;
    this.createdAt = data.createdAt;
  }
}
