interface Config {
  id: number;
  nomMatch: string;
  nombreQuartTemps: string;
  dureeQuartTemps: string; // Durée en secondes
  dureeTimeout: string; // Durée en secondes
  dateHeure: Date;
  userId: number;
  user: any;
  equipeDomicileId?: number; // ID de l'équipe domicile
  equipeExterieurId?: number; // ID de l'équipe extérieur
  equipeDomicileNom?: string; // Nom de l'équipe domicile
  equipeExterieurNom?: string; // Nom de l'équipe extérieur
}

export default Config;
