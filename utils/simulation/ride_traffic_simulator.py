#!/usr/bin/env python3
"""
Ride Traffic Simulator Script (API Version)
Generates ride entry/exit records for real-time traffic visualization testing using API calls.

This script simulates visitors entering and exiting rides to generate realistic
traffic data for testing the ride traffic visualization feature by calling the REST API.
"""

import requests
import random
import time
import argparse
import logging
from datetime import datetime, timedelta
from typing import List, Tuple, Optional, Dict
import threading
import json

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

class RideTrafficSimulatorAPI:
    def __init__(self, base_url: str, num_visitors: int = 100, cleanup: bool = False, force_cleanup: bool = False):
        """
        Initialize the ride traffic simulator using API calls.
        
        Args:
            base_url: Base URL of the API (e.g., http://localhost:5036)
            num_visitors: Number of test visitors to create
            cleanup: Whether to clean up visitors after simulation
            force_cleanup: Whether to force cleanup without confirmation
        """
        self.base_url = base_url.rstrip('/')
        self.num_visitors = num_visitors
        self.cleanup = cleanup
        self.force_cleanup = force_cleanup
        self.active_entries = []  # Track active ride entries without exit times
        self.visitor_ids = []
        self.ride_ids = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15]  # From seeding
        self.session = requests.Session()
        
    def create_test_visitors(self) -> List[int]:
        """
        Create test visitors for simulation using the API.
        
        Returns:
            List of created visitor IDs
        """
        visitor_ids = []
        now = datetime.utcnow()
        
        # Get the maximum existing visitor ID to avoid conflicts
        max_visitor_id = 10000  # Start from a high number to avoid conflicts
        
        for i in range(self.num_visitors):
            # Create visitor data
            visitor_data = {
                "username": f"visitor_{i+1}_{int(time.time())}",
                "passwordHash": "hashedpassword123",
                "email": f"visitor{i+1}_{int(time.time())}@test.com",
                "displayName": f"Test Visitor {i+1}",
                "phoneNumber": f"1234567{i+1:03d}",
                "birthDate": (now - timedelta(days=random.randint(365*18, 365*80))).isoformat(),
                "gender": random.choice(["Male", "Female"]),
                "visitorType": "Regular",
                "height": random.randint(120, 200)
            }
            
            # Create visitor via API
            try:
                response = self.session.post(
                    f"{self.base_url}/api/user/visitors",
                    json=visitor_data,
                    headers={"Content-Type": "application/json"}
                )
                
                if response.status_code == 201:
                    result = response.json()
                    visitor_id = result.get("visitorId")
                    if visitor_id is not None:
                        visitor_ids.append(visitor_id)
                        logger.info(f"Created visitor {visitor_id}")
                    else:
                        logger.warning(f"No visitorId returned for visitor {i+1}")
                else:
                    logger.warning(f"Failed to create visitor {i+1}: {response.status_code} - {response.text}")
                    
            except Exception as e:
                logger.error(f"Error creating visitor {i+1}: {e}")
                
        logger.info(f"Created {len(visitor_ids)} test visitors")
        return visitor_ids
        
    def get_active_entries(self, limit: int = 50) -> List[Dict]:
        """
        Get recent active ride entries (entries without exit times) from the API.
        
        Args:
            limit: Maximum number of entries to retrieve
            
        Returns:
            List of active entries
        """
        try:
            response = self.session.get(f"{self.base_url}/api/user/ride-entries")
            if response.status_code == 200:
                all_entries = response.json()
                # Filter for entries without exit times
                active_entries = [entry for entry in all_entries if entry.get("exitTime") is None]
                return active_entries[:limit]
        except Exception as e:
            logger.error(f"Error getting active entries: {e}")
            
        return []
        
    def generate_ride_entry(self) -> Optional[int]:
        """
        Generate a new ride entry record using the API.
        
        Returns:
            RideEntryRecordId of the created entry, or None if failed
        """
        if not self.visitor_ids or not self.ride_ids:
            logger.warning("No visitors or rides available")
            return None
            
        # Select random visitor and ride
        visitor_id = random.choice(self.visitor_ids)
        ride_id = random.choice(self.ride_ids)
        
        # Select random entry gate
        entry_gates = [f"Gate_{i}" for i in range(1, 11)]
        entry_gate = random.choice(entry_gates)
        
        # Create ride entry data
        entry_data = {
            "visitorId": visitor_id,
            "rideId": ride_id,
            "type": "entry",
            "gateName": entry_gate
        }
        
        # Create ride entry via API
        try:
            response = self.session.post(
                f"{self.base_url}/api/user/ride-entries",
                json=entry_data,
                headers={"Content-Type": "application/json"}
            )
            
            if response.status_code == 201:
                result = response.json()
                record_id = result.get("RideEntryRecordId")
                logger.info(f"Generated entry: Visitor {visitor_id} entered Ride {ride_id}")
                return record_id
            else:
                logger.warning(f"Failed to create ride entry: {response.status_code} - {response.text}")
                
        except Exception as e:
            logger.error(f"Error creating ride entry: {e}")
            
        return None
        
    def generate_ride_exit(self, record_id: int = None) -> bool:
        """
        Generate a ride exit record for an existing entry using the API.
        
        Args:
            record_id: Specific record ID to exit, or None for random
            
        Returns:
            True if exit was generated, False otherwise
        """
        if record_id is None:
            # Get a random active entry
            active_entries = self.get_active_entries(20)
            if not active_entries:
                logger.debug("No active entries found for exit")
                return False
                
            entry = random.choice(active_entries)
            record_id = entry.get("rideEntryRecordId")
            visitor_id = entry.get("visitorId")
            ride_id = entry.get("rideId")
        else:
            # For now, we'll need to get the entry details differently
            visitor_id = 0
            ride_id = 0
            
        # Select random exit gate
        exit_gates = [f"Exit_{i}" for i in range(1, 6)]
        exit_gate = random.choice(exit_gates)
        
        # Generate exit time (between 30 seconds and 15 minutes from now)
        exit_time = datetime.utcnow() + timedelta(seconds=random.randint(30, 900))
        
        # Update ride entry record with exit time
        exit_data = {
            "rideEntryRecordId": record_id,
            "exitGate": exit_gate,
            "exitTime": exit_time.isoformat()
        }
        
        try:
            response = self.session.put(
                f"{self.base_url}/api/user/ride-entries/{record_id}",
                json=exit_data,
                headers={"Content-Type": "application/json"}
            )
            
            if response.status_code == 200:
                logger.info(f"Generated exit: Visitor {visitor_id} exited Ride {ride_id}")
                return True
            else:
                logger.warning(f"Failed to update ride entry for exit: {response.status_code} - {response.text}")
                
        except Exception as e:
            logger.error(f"Error updating ride entry for exit: {e}")
            
        return False
        
    def delete_visitor(self, visitor_id: int) -> bool:
        """
        Delete a visitor using the API.
        
        Args:
            visitor_id: ID of the visitor to delete
            
        Returns:
            True if deletion was successful, False otherwise
        """
        try:
            response = self.session.delete(f"{self.base_url}/api/user/visitors/{visitor_id}")
            
            if response.status_code == 200:
                logger.info(f"Deleted visitor {visitor_id}")
                return True
            else:
                logger.warning(f"Failed to delete visitor {visitor_id}: {response.status_code} - {response.text}")
                return False
                
        except Exception as e:
            logger.error(f"Error deleting visitor {visitor_id}: {e}")
            return False
    
    def cleanup_visitors(self):
        """
        Clean up visitors created during the simulation.
        """
        if not self.cleanup:
            logger.info("Cleanup disabled, skipping visitor deletion")
            return
            
        if not self.visitor_ids:
            logger.warning("No visitors to clean up")
            return
            
        if not self.force_cleanup:
            response = input(f"Delete {len(self.visitor_ids)} visitors created during simulation? (y/N): ")
            if response.lower() != 'y':
                logger.info("Visitor cleanup cancelled by user")
                return
        
        logger.info(f"Cleaning up {len(self.visitor_ids)} visitors...")
        deleted_count = 0
        
        for visitor_id in self.visitor_ids:
            if self.delete_visitor(visitor_id):
                deleted_count += 1
            # Add a small delay to avoid overwhelming the API
            time.sleep(0.1)
            
        logger.info(f"Cleanup completed. Deleted {deleted_count}/{len(self.visitor_ids)} visitors")
        
    def run_simulation(self, duration_minutes: int = 10, entries_per_minute: int = 5):
        """
        Run the ride traffic simulation using API calls.
        
        Args:
            duration_minutes: How long to run the simulation
            entries_per_minute: How many new entries to generate per minute
        """
        logger.info(f"Starting ride traffic simulation for {duration_minutes} minutes")
        logger.info(f"Generating {entries_per_minute} entries per minute")
        
        end_time = datetime.utcnow() + timedelta(minutes=duration_minutes)
        
        while datetime.utcnow() < end_time:
            try:
                # Generate new entries
                for _ in range(entries_per_minute):
                    self.generate_ride_entry()
                    
                # Generate some exits (50-80% of the time)
                if random.random() < 0.7:
                    self.generate_ride_exit()
                    
                # Wait for a second before next iteration
                time.sleep(1)
                
            except KeyboardInterrupt:
                logger.info("Simulation interrupted by user")
                break
            except Exception as e:
                logger.error(f"Error during simulation: {e}")
                
        logger.info("Simulation completed")

def main():
    parser = argparse.ArgumentParser(description="Ride Traffic Simulator (API Version)")
    parser.add_argument("--base-url", default="http://localhost:5036",
                        help="Base URL of the API (default: http://localhost:5036)")
    parser.add_argument("--visitors", type=int, default=100,
                        help="Number of test visitors to create (default: 100)")
    parser.add_argument("--duration", type=int, default=10,
                        help="Simulation duration in minutes (default: 10)")
    parser.add_argument("--entries-per-minute", type=int, default=5,
                        help="Number of new entries per minute (default: 5)")
    parser.add_argument("--cleanup", action="store_true",
                        help="Clean up visitors after simulation")
    parser.add_argument("--force-cleanup", action="store_true",
                        help="Force cleanup without confirmation")
    
    args = parser.parse_args()
    
    # Create simulator
    simulator = RideTrafficSimulatorAPI(args.base_url, args.visitors, args.cleanup, args.force_cleanup)
    
    try:
        # Create test visitors
        logger.info("Creating test visitors...")
        simulator.visitor_ids = simulator.create_test_visitors()
        
        if not simulator.visitor_ids:
            logger.warning("No visitors created, using default visitor IDs")
            # Use some default visitor IDs that should exist in the database
            simulator.visitor_ids = list(range(1, min(101, args.visitors + 1)))
        
        # Run simulation
        simulator.run_simulation(args.duration, args.entries_per_minute)
        
    except Exception as e:
        logger.error(f"Simulation failed: {e}")
    finally:
        # Clean up visitors if requested
        simulator.cleanup_visitors()

if __name__ == "__main__":
    main()
