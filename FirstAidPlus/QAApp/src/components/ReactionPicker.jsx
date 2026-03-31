import React, { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Smile } from 'lucide-react';

const reactions = ['👍', '❤️', '😂', '😮', '😢'];

const ReactionPicker = ({ currentReaction, onSelect }) => {
    const [isOpen, setIsOpen] = useState(false);

    return (
        <div className="position-relative" onMouseEnter={() => setIsOpen(true)} onMouseLeave={() => setIsOpen(false)}>
            <button 
                className={`btn btn-sm ${currentReaction ? 'btn-primary' : 'btn-outline-secondary'} rounded-circle d-flex align-items-center justify-content-center`}
                style={{ width: '28px', height: '28px', fontSize: '12px', padding: 0 }}
                onClick={() => currentReaction && onSelect(currentReaction)}
            >
                {currentReaction || <Smile size={16} />}
            </button>

            <AnimatePresence>
                {isOpen && (
                    <motion.div
                        initial={{ opacity: 0, y: 10, scale: 0.8 }}
                        animate={{ opacity: 1, y: -40, scale: 1 }}
                        exit={{ opacity: 0, y: 10, scale: 0.8 }}
                        className="position-absolute bg-white shadow rounded-pill p-1 d-flex gap-1 border"
                        style={{ left: '-10px', zIndex: 100 }}
                    >
                        {reactions.map(emoji => (
                            <button
                                key={emoji}
                                className="btn btn-sm btn-light border-0 p-1"
                                style={{ fontSize: '1.2rem', transition: 'transform 0.1s' }}
                                onClick={() => {
                                    onSelect(emoji);
                                    setIsOpen(false);
                                }}
                                onMouseEnter={(e) => e.target.style.transform = 'scale(1.3)'}
                                onMouseLeave={(e) => e.target.style.transform = 'scale(1)'}
                            >
                                {emoji}
                            </button>
                        ))}
                    </motion.div>
                )}
            </AnimatePresence>
        </div>
    );
};

export default ReactionPicker;
