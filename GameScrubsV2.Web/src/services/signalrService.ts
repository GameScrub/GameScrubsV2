import * as signalR from '@microsoft/signalr';
import { API_BASE_URL } from '@/api.config';

export type MatchUpdateCallback = (data: { bracketId: number; matchId: number }) => void;
export type StatusChangeCallback = (data: { bracketId: number; newStatus: string }) => void;

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private currentBracketId: number | null = null;

  /**
   * Initialize the SignalR connection
   */
  async startConnection(): Promise<void> {
    if (this.connection) {
      return; // Already connected
    }

    // Construct hub URL - remove /api suffix if present since hub is at root level
    const baseUrl = API_BASE_URL.endsWith('/api')
      ? API_BASE_URL.slice(0, -4)
      : API_BASE_URL;
    const hubUrl = `${baseUrl}/hubs/bracket`;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        skipNegotiation: false,
        transport:
          signalR.HttpTransportType.WebSockets |
          signalR.HttpTransportType.ServerSentEvents |
          signalR.HttpTransportType.LongPolling,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    try {
      await this.connection.start();
      console.log('SignalR connected');
    } catch (err) {
      console.error('Error starting SignalR connection:', err);
      throw err;
    }
  }

  /**
   * Stop the SignalR connection
   */
  async stopConnection(): Promise<void> {
    if (this.connection) {
      try {
        await this.connection.stop();
        console.log('SignalR disconnected');
      } catch (err) {
        console.error('Error stopping SignalR connection:', err);
      }
      this.connection = null;
      this.currentBracketId = null;
    }
  }

  /**
   * Join a bracket room to receive updates for a specific bracket
   */
  async joinBracket(bracketId: number): Promise<void> {
    if (!this.connection) {
      await this.startConnection();
    }

    if (this.currentBracketId === bracketId) {
      return; // Already in this bracket
    }

    // Leave previous bracket if any
    if (this.currentBracketId !== null) {
      await this.leaveBracket(this.currentBracketId);
    }

    try {
      await this.connection!.invoke('JoinBracket', bracketId.toString());
      this.currentBracketId = bracketId;
      console.log(`Joined bracket ${bracketId}`);
    } catch (err) {
      console.error(`Error joining bracket ${bracketId}:`, err);
      throw err;
    }
  }

  /**
   * Leave a bracket room
   */
  async leaveBracket(bracketId: number): Promise<void> {
    if (this.connection) {
      try {
        await this.connection.invoke('LeaveBracket', bracketId.toString());
        console.log(`Left bracket ${bracketId}`);
        if (this.currentBracketId === bracketId) {
          this.currentBracketId = null;
        }
      } catch (err) {
        console.error(`Error leaving bracket ${bracketId}:`, err);
      }
    }
  }

  /**
   * Register a callback for when a match score is updated
   */
  onMatchScoreUpdated(callback: MatchUpdateCallback): void {
    if (!this.connection) {
      console.warn('Cannot register onMatchScoreUpdated: Connection not established');
      return;
    }
    this.connection.on('MatchScoreUpdated', callback);
  }

  /**
   * Register a callback for when the bracket status changes
   */
  onBracketStatusChanged(callback: StatusChangeCallback): void {
    if (!this.connection) {
      console.warn('Cannot register onBracketStatusChanged: Connection not established');
      return;
    }
    this.connection.on('BracketStatusChanged', callback);
  }

  /**
   * Remove all event handlers (useful for cleanup)
   */
  offAll(): void {
    if (this.connection) {
      this.connection.off('MatchScoreUpdated');
      this.connection.off('BracketStatusChanged');
    }
  }

  /**
   * Get the connection state
   */
  getConnectionState(): signalR.HubConnectionState | null {
    return this.connection?.state ?? null;
  }
}

// Export a singleton instance
export const signalrService = new SignalRService();
